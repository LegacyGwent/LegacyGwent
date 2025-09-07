using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.Localization;
using Autofac;
using UnityEngine.AddressableAssets;
using Cynthia.Card;

public class MyCards : MonoBehaviour
{
    //logic
    private LocalizationService translator;

    [Header("Fields")]
    public Text Strength;
    public Image FactionIcon;
    public Image CardBorder;
    public Image CardImg;

    [Header("Sprites")]
    public Sprite CopperBorder;
    public Sprite SilverBorder;
    public Sprite GoldBorder;
    //public Sprite NorthernRealmsNormalIcon;
    //public Sprite ScoiaTaelNormalIcon;
    //public Sprite MonstersNormalIcon;
    //public Sprite SkelligeNormalIcon;
    //public Sprite NilfgaardNormalIcon;
    public Sprite NeutralNormalIcon;
    public Sprite NorthernRealmsGoldIcon;
    public Sprite ScoiaTaelGoldIcon;
    public Sprite MonstersGoldIcon;
    public Sprite SkelligeGoldIcon;
    public Sprite NilfgaardGoldIcon;
    public Sprite NeutralGoldIcon;

    void Start()
    {
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
}