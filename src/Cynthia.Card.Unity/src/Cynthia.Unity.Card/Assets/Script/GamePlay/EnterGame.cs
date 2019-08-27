using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterGame : MonoBehaviour
{
    void Start()
    {
        AdaptiveResolution();
    }
    public void AdaptiveResolution()
    {
#if !UNITY_ANDROID
        var scale = 1920f / Screen.width;
        var scale2 = Screen.height / 1080f * scale;
        transform.localScale = Vector3.one / scale2;
#endif
#if UNITY_ANDROID
        // var scale = 1920f / Screen.width;
        // var scale2 = Screen.height / 1080f * scale;
        // transform.localScale = Vector3.one / scale2;
        // var scale = 1080f / Screen.height;
        // var scale2 = Screen.width / 1920f * scale;
        // transform.localScale = Vector3.one / scale2;
#endif
    }
    private void Update()
    {
        // AdaptiveResolution();
    }
}
