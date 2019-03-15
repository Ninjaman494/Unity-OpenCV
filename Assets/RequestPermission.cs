using UnityEngine;
using UnityEngine.Android;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class RequestPermission : MonoBehaviour {

    [DllImport("SharedObject1", EntryPoint = "doTracking")]
    private static extern int doTracking();


    void Start() {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
            Permission.RequestUserPermission(Permission.Camera);
        } 
#endif
        //Debug.Log("Result: " + doTracking());
    }
}