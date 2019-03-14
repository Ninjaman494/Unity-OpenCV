using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using NatCamU.Core;

public class NATNativeCode : MonoBehaviour {
    [DllImport("SharedObject1", EntryPoint = "sendRawImageBytes")]
    private static extern int sendRawImageBytes(Color32[] raw, int width, int height);
    [DllImport("SharedObject1", EntryPoint = "getRawImageBytes")]
    private static extern void getRawImageBytes(IntPtr data, int width, int height);

    private Texture2D tex;
    private Color32[] pixel32;

    private GCHandle pixelHandle;
    private IntPtr pixelPtr;

    public RawImage rawImage;
    public AspectRatioFitter aspectFitter;

    private Boolean inited = false;

    void Start() {
        NatCam.Play(DeviceCamera.RearCamera);
        NatCam.OnStart += OnStart;
        //gameObject.GetComponent<Renderer>().material.mainTexture = tex;
    }

    void OnStart() {
        InitTexture();
        inited = true;
        
        Texture2D texture = NatCam.Preview as Texture2D;
        Color32[] rawImg = texture.GetPixels32();
        Debug.Log("ri:" + rawImg.GetValue(0));
        sendRawImageBytes(rawImg, NatCam.Preview.width,NatCam.Preview.height);

        //rawImage.texture = NatCam.Preview;
        rawImage.texture = tex;
        aspectFitter.aspectRatio = NatCam.Preview.width / (float)NatCam.Preview.height;
    }

    void Update() {
        //Debug.Log("called");
        if(inited) {
            Texture2D texture = NatCam.Preview as Texture2D;
            Color32[] rawImg = texture.GetPixels32();
            Debug.Log("ri:" + rawImg.GetValue(0));
            sendRawImageBytes(rawImg, NatCam.Preview.width,NatCam.Preview.height);

            Debug.Log("called");
            MatToTexture2D();
        }
    }
    void InitTexture() {
        tex = new Texture2D(NatCam.Preview.width, NatCam.Preview.height, TextureFormat.RGBA32, false);
        pixel32 = tex.GetPixels32();
        pixelHandle = GCHandle.Alloc(pixel32, GCHandleType.Pinned); // Pin pixel32 array
        pixelPtr = pixelHandle.AddrOfPinnedObject(); // Get the pinned address
    }

    void MatToTexture2D() {
        //Convert Mat to Texture2D
        Debug.Log("Calling c++");
        getRawImageBytes(pixelPtr, tex.width, tex.height);
        Debug.Log("Called c++");

        //Update the Texture2D with array updated in C++
        System.Array.Reverse(pixel32);
        tex.SetPixels32(pixel32);
        tex.Apply();
    }

    void OnApplicationQuit() {
        //pixelHandle.Free();
    }
}
