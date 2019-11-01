using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTest : MonoBehaviour
{
    public Image Image;
    public Sprite Sprite1;
    public Sprite Sprite2;

    public IDictionary<int, Sprite> Sprites;

    private int _index = 0;

    public void Start()
    {
        Sprites = new Dictionary<int, Sprite>();
        Sprites[0] = Sprite1;
        Sprites[1] = Sprite2;
    }

    public void Change()
    {
        _index = 1 - _index;
        Image.sprite = Sprites[_index];
    }
}
