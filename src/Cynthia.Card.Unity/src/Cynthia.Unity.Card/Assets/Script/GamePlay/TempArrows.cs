using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempArrows : MonoBehaviour
{
    public RectTransform Arrows;
    public Transform GameAll;
    public void SetArrows(Vector3 source,Vector3 taget,bool isLight)
    {
        transform.up = (taget - source).normalized;
        transform.position = (source + taget) / 2;
        //var scaleX = GameAll.localScale.x*0.46745f*0.02f;
        //var scaleY = GameAll.localScale.y * 0.550055f * 0.02f;
        //var trueTaget = new Vector2(taget.x * scaleX, taget.y * scaleY);
        //var trueSource = new Vector2(source.x * scaleX, source.y * scaleY);
        var distance = Vector2.Distance(taget,source);

        Arrows.sizeDelta = new Vector2(Arrows.sizeDelta.x,distance*(100f/GameAll.localScale.x));
        //Debug.Log(distance);
    }
}
