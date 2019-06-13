using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Autofac;
using Cynthia.Card;

public class DeckShowInfo : MonoBehaviour
{
    public Text DeckText;
    public Sprite UnAvaliableIcon;
    public Sprite AvaliableIcon;
    public Image HeadIcon;
    public Image AvaliableShow;

    public void SetDeckInfo(string name,bool isAvaliable)
    {
        AvaliableShow.sprite = isAvaliable ? AvaliableIcon : UnAvaliableIcon;
        DeckText.text = name;
    }
}
