using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoResolution : MonoBehaviour
{
    public CanvasScaler[] Targets;

    void Update()
    {
        if (((float)Screen.width / (float)Screen.height) <= (1920f / 1080f))
        {
            foreach (var target in Targets)
            {
                target.matchWidthOrHeight = 0;
            }
        }
        else
        {
            foreach (var target in Targets)
            {
                target.matchWidthOrHeight = 1;
            }
        }
    }
}
