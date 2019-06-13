using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLoadImage : MonoBehaviour
{
	void Start ()
    {
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Cards/6010200");
	}
}
