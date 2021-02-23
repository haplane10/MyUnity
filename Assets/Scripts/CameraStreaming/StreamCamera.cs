using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using LogicServer;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreamCamera : MonoBehaviour
{
    [SerializeField] UnityClient client;
    [SerializeField] new Camera camera;
    public int bytesPerPixel;
    private byte[] textureByteData;

    [SerializeField] Material targetMaterial;
    [SerializeField] Texture2D texture2D;
    [SerializeField] float fps;
    [SerializeField] int maxPacketSize;
    // Start is called before the first frame update
    void Start()
    {
        client.MessageReceived += MessageReceived;

        StartCoroutine(GetCameraTexture());
    }

    void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            using (DarkRiftReader reader = message.GetReader())
            {
                switch (message.Tag)
                {
                    //////////////////////////////////////////////////////////////////////////////
                    //// LOGIC
                    case MessageTags.VIDEO_RESOLUTION:
                        {
                            uint checkflag = reader.ReadUInt32();
                            int width = reader.ReadInt32();
                            int height = reader.ReadInt32();

                            camera.targetTexture = new RenderTexture(width, height, 0);

                            Debug.Log($"W : {width}, H: {height}");
                        }
                        break;
                }
            }
        }
    }

    public Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, true);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 아래 부분은 DarkRift 전송 전 Byte[] 압축후 전송하는 부분 구현 (압축후에도 사이즈가 클경우 분할해서 보내야함)
    /// </summary>
    public void SendCameraTexture(int numOfDatas, int idx, byte[] data)
    {
        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            short uid = 1;
            writer.Write((byte)0);
            writer.Write(uid);
            writer.Write(numOfDatas);
            writer.Write(idx);
            writer.Write(data);

            using (Message message = Message.Create(MessageTags.VIDEO_STREAMING, writer))
                client.SendMessage(message, SendMode.Reliable);
        }
    }

    /// GZipStream byte[] 압축한것
    //IEnumerator GetCameraTexture()
    //{
    //    yield return new WaitForEndOfFrame();

    //    while (true)
    //    {
    //        texture2D = ToTexture2D(camera.targetTexture);
    //        targetMaterial.mainTexture = texture2D;

    //        var datas = texture2D.GetRawTextureData();

    //        MemoryStream ms = new MemoryStream();
    //        GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
    //        zip.Write(datas, 0, datas.Length);
    //        zip.Close();
    //        ms.Position = 0;

    //        // Byte[] 압축
    //        MemoryStream output = new MemoryStream();
    //        byte[] compressed = new byte[ms.Length];
    //        ms.Read(compressed, 0, compressed.Length);

    //        byte[] gzBuffer = new byte[compressed.Length + 4];
    //        Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
    //        Buffer.BlockCopy(BitConverter.GetBytes(datas.Length), 0, gzBuffer, 0, 4);

    //        Debug.Log($"data size = {datas.Length},  compress size = {gzBuffer.Length}");

    //        var compData = gzBuffer;
    //        var numOfDatas = compData.Length / maxPacketSize;
    //        var remainOfDatas = compData.Length % maxPacketSize;

    //        if (compData.Length > maxPacketSize)
    //        {
    //            for (int idx = 0; idx <= numOfDatas; idx++)
    //            {
    //                List<byte> newData = new List<byte>();
    //                for (int i = (idx * maxPacketSize); i < ((idx != numOfDatas) ? ((idx + 1) * maxPacketSize) : (idx * maxPacketSize) + remainOfDatas); i++)
    //                {
    //                    newData.Add(compData[i]);
    //                }
    //                SendCameraTexture(numOfDatas, idx, newData.ToArray());
    //            }
    //        }
    //        else
    //        {
    //            SendCameraTexture(numOfDatas, 0, compData);
    //        }

    //        yield return new WaitForSeconds(1f / fps);
    //    }
    //}

    /// DeflateStream으로 byte[] 압축한것
    IEnumerator GetCameraTexture()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            texture2D = ToTexture2D(camera.targetTexture);
            targetMaterial.mainTexture = texture2D;

            var datas = texture2D.GetRawTextureData();

            // Byte[] 압축
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                dstream.Write(datas, 0, datas.Length);
            }
            Debug.Log($"data size = {datas.Length},  compress size = {output.ToArray().Length}");

            var compData = output.ToArray();
            var numOfDatas = compData.Length / maxPacketSize;
            var remainOfDatas = compData.Length % maxPacketSize;

            if (compData.Length > maxPacketSize)
            {
                for (int idx = 0; idx <= numOfDatas; idx++)
                {
                    List<byte> newData = new List<byte>();
                    for (int i = (idx * maxPacketSize); i < ((idx != numOfDatas) ? ((idx + 1) * maxPacketSize) : (idx * maxPacketSize) + remainOfDatas); i++)
                    {
                        newData.Add(compData[i]);
                    }
                    SendCameraTexture(numOfDatas, idx, newData.ToArray());
                }
            }
            else
            {
                SendCameraTexture(numOfDatas, 0, compData);
            }

            yield return new WaitForSeconds(1f / fps);
        }
    }

    public void SetCameraTexture()
    {
        var newTexture2D = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        newTexture2D.LoadRawTextureData(textureByteData);
        newTexture2D.Apply();

        targetMaterial.mainTexture = newTexture2D;
    }

    ////////////////////////////////
    /// <summary>
    /// 아래 부분은 DarkRift Packet사이즈보다 전송 byte[]가 클경우 분할해서 전송하는 부분을 구현한것
    /// </summary>
    //public void SendCameraTexture(int numOfDatas, int idx, byte[] data)
    //{
    //    using (DarkRiftWriter writer = DarkRiftWriter.Create())
    //    {
    //        short uid = 1;
    //        writer.Write((byte)0);
    //        writer.Write(uid);
    //        writer.Write(numOfDatas);
    //        writer.Write(idx);
    //        writer.Write(data);

    //        using (Message message = Message.Create(MessageTags.VIDEO_STREAMING, writer))
    //            client.SendMessage(message, SendMode.Reliable);
    //    }
    //}
    //IEnumerator GetCameraTexture()
    //{
    //    yield return new WaitForEndOfFrame();

    //    while (true)
    //    {
    //        texture2D = ToTexture2D(camera.targetTexture);
    //        targetMaterial.mainTexture = texture2D;
    //        var datas = texture2D.GetRawTextureData();
    //        var numOfDatas = datas.Length / maxPacketSize;
    //        var remainOfDatas = datas.Length % maxPacketSize;

    //        if (datas.Length > maxPacketSize)
    //        {
    //            for (int idx = 0; idx <= numOfDatas; idx++)
    //            {
    //                List<byte> newData = new List<byte>();
    //                for (int i = (idx * maxPacketSize); i < ((idx != numOfDatas) ? ((idx + 1) * maxPacketSize) : (idx * maxPacketSize) + remainOfDatas); i++)
    //                {
    //                    newData.Add(datas[i]);
    //                }
    //                SendCameraTexture(numOfDatas, idx, newData.ToArray());
    //            }
    //        }
    //        else
    //        {
    //            SendCameraTexture(numOfDatas, 0, datas);
    //        }
    //        yield return new WaitForSeconds(1f / fps);
    //    }
    //}
    ////////////////////////////

    ////////////////////////////
    /// <summary>
    /// 아래 부분은 DarkRift에 바로 전송하는 부분 구현 (Packet Size 보다 클경우 에러 발생)
    /// </summary>
    //public void SendCameraTexture()
    //{
    //    using (DarkRiftWriter writer = DarkRiftWriter.Create())
    //    {
    //        short uid = 1;
    //        writer.Write((byte)0);
    //        writer.Write(uid);
    //        writer.Write(texture2D.GetRawTextureData());

    //        using (Message message = Message.Create(MessageTags.VIDEO_STREAMING, writer))
    //            client.SendMessage(message, SendMode.Reliable);

    //        Debug.Log("Send Texture Data");
    //    }
    //}
    //IEnumerator GetCameraTexture()
    //{
    //    yield return new WaitForEndOfFrame();

    //    while (true)
    //    {
    //        texture2D = ToTexture2D(camera.targetTexture);
    //        targetMaterial.mainTexture = texture2D;
    //        Debug.Log(texture2D.GetRawTextureData().Length);

    //        SendCameraTexture();
    //        yield return new WaitForSeconds(1f / fps);
    //    }
    //}
    /////////////////////////
}
