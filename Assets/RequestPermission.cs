using UnityEngine;
using UnityEngine.Android;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class RequestPermission : MonoBehaviour {
    void Start() {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
            Permission.RequestUserPermission(Permission.Camera);
        } 
#endif
    }
}