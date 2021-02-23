using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using LogicServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

public class StreamManager : MonoBehaviour
{
    [SerializeField] private UnityClient client;
    private Texture2D newTexture2D;
    private int width = 160, height = 90;
    //private byte[] textureByteData;
    [SerializeField] Material targetMaterial;
    List<byte> textureByteData = new List<byte>();

    void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            using (DarkRiftReader reader = message.GetReader())
            {
                switch (message.Tag)
                {
                    //// Byte[] 압축 수신일 경우
                    //case MessageTags.VIDEO_STREAMING:
                    //    {
                    //        uint checkflag = reader.ReadUInt32();
                    //        short uid = reader.ReadInt16();

                    //        MemoryStream input = new MemoryStream(reader.ReadBytes());
                    //        MemoryStream output = new MemoryStream();
                    //        using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                    //        {
                    //            dstream.CopyTo(output);
                    //        }
                    //        SetCameraTexture(output.ToArray());
                    //    }
                    //    break;

                    //// LOGIC  (DeflateStream Byte[]분할 수신일 경우)
                    case MessageTags.VIDEO_STREAMING:
                        {
                            uint checkflag = reader.ReadUInt32();
                            short uid = reader.ReadInt16();
                            int numOfDatas = reader.ReadInt32();
                            var idx = reader.ReadInt32();

                            if (idx == 0)
                            {
                                textureByteData.Clear();
                            }

                            textureByteData.AddRange(reader.ReadBytes());

                            if (idx == numOfDatas)
                            {
                                Debug.Log(textureByteData.Count);
                                MemoryStream input = new MemoryStream(textureByteData.ToArray());
                                MemoryStream output = new MemoryStream();
                                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                                {
                                    dstream.CopyTo(output);
                                }
                                SetCameraTexture(output.ToArray());
                            }
                        }
                        break;

                        //// LOGIC  (BitConverter Byte[]분할 수신일 경우)
                        //case MessageTags.VIDEO_STREAMING:
                        //    {
                        //        uint checkflag = reader.ReadUInt32();
                        //        short uid = reader.ReadInt16();
                        //        int numOfDatas = reader.ReadInt32();
                        //        var idx = reader.ReadInt32();

                        //        if (idx == 0)
                        //        {
                        //            textureByteData.Clear();
                        //        }

                        //        textureByteData.AddRange(reader.ReadBytes());

                        //        if (idx == numOfDatas)
                        //        {
                        //            MemoryStream ms = new MemoryStream();
                        //            var gzBuffer = textureByteData.ToArray();
                        //            int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                        //            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                        //            byte[] buffer = new byte[msgLength];

                        //            ms.Position = 0;
                        //            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
                        //            zip.Read(buffer, 0, buffer.Length);

                        //            SetCameraTexture(buffer);
                        //        }
                        //    }
                        //    break;
                }
            }
        }
    }

    public void SetCameraTexture(byte[] textureByteData)
    {
        newTexture2D = new Texture2D(width, height, TextureFormat.RGB24, true);
        newTexture2D.LoadRawTextureData(textureByteData);
        newTexture2D.Apply();

        targetMaterial.mainTexture = newTexture2D;
    }

    // Start is called before the first frame update
    void Start()
    {
        client.MessageReceived += MessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVidioResolution(int idx)
    {
        switch(idx)
        {
            case 1:
                SendVideoResolutionData(160, 90);
                break;
            case 2:
                SendVideoResolutionData(320, 180);
                break;
            case 3:
                SendVideoResolutionData(480, 270);
                break;
            case 4:
                SendVideoResolutionData(720, 480);
                break;
            case 5:
                SendVideoResolutionData(1280, 720);
                break;
            default:
                break;
        }
    }

    private void SendVideoResolutionData(int _width, int _height)
    {
        width = _width;
        height = _height;

        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            writer.Write((byte)0);
            writer.Write(width);
            writer.Write(height);

            using (Message message = Message.Create(MessageTags.VIDEO_RESOLUTION, writer))
                client.SendMessage(message, SendMode.Reliable);

            Debug.Log("Send Texture Resolution Data");
        }
    }

}
