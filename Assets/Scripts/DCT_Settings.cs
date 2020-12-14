using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DCT;

public class DCT_Settings : Singleton<DCT_Settings>
{
    private DCT.PlayMode playMode = DCT.PlayMode.PC;
    public DCT.PlayMode PlayMode
    {
        get
        {
            return playMode;
        }
    }

    void Awake()
    {
        if (UnityEngine.XR.XRSettings.enabled)
            playMode = DCT.PlayMode.VR;
        else
            playMode = DCT.PlayMode.PC;
    }
}
