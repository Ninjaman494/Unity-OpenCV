using UnityEngine;
using UnityEngine.Android;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class CallNativeCode : MonoBehaviour {
    [DllImport("SharedObject1", EntryPoint = "sendRawImageBytes")]
    private static extern int sendRawImageBytes(Color32[] raw, int width, int height);
    [DllImport("SharedObject1", EntryPoint = "getRawImageBytes")]
    private static extern void getRawImageBytes(IntPtr data, int width, int height);
 
    private WebCamTexture webcam;

    private Texture2D tex;
    private Color32[] pixel32;

    private GCHandle pixelHandle;
    private IntPtr pixelPtr;


    void Start() {
        var devices = WebCamTexture.devices;
        webcam = new WebCamTexture(devices[0].name);
        webcam.Play();

        InitTexture();
        gameObject.GetComponent<Renderer>().material.mainTexture = tex;
    }

    void Update() {
        Debug.Log("called");
        MatToTexture2D();
    }

    void OnGUI () {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
            GUI.Label(new Rect(15, 125, 450, 100), "Didn't get camera permission");
        } else {
            Color32[] rawImg = webcam.GetPixels32();
            GUI.Label(new Rect(15, 125, 450, 100), "videoStream: " + sendRawImageBytes(rawImg,webcam.width,webcam.height));
        }
#endif
    }

    void InitTexture() {
        tex = new Texture2D(512, 512, TextureFormat.RGBA32, false);
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
        pixelHandle.Free();
    }
}
