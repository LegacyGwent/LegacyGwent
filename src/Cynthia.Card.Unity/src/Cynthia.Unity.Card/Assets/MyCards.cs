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
    //private LocalizationService translator;

    [Header("Fields")]
    public Text Strength;
    public Image FactionIcon;
    public Image CardBorder;
    public Image CardImg;

    [Header("Sprites")]
    public Sprite CopperBorder;
    public Sprite SilverBorder;
    public Sprite GoldBorder;
    public Sprite NorthernRealmsNormalIcon;
    public Sprite ScoiaTaelNormalIcon;
    public Sprite MonstersNormalIcon;
    public Sprite SkelligeNormalIcon;
    public Sprite NilfgaardNormalIcon;
    public Sprite NeutralNormalIcon;
    public Sprite NorthernRealmsGoldIcon;
    public Sprite ScoiaTaelGoldIcon;
    public Sprite MonstersGoldIcon;
    public Sprite SkelligeGoldIcon;
    public Sprite NilfgaardGoldIcon;
    public Sprite NeutralGoldIcon;

    void Start()
    {
        //translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    public void SetCard(string CardId)
    {
        Debug.Log("xxxxxxx "+CardId);
        
        var CardInfo = GwentMap.CardMap[CardId];

        Strength.text = (CardInfo.Strength).ToString();
        SetFaction(FactionIcon,CardInfo);
        SetBorder(CardBorder,CardInfo);
        Addressables.LoadAssetAsync<Sprite>(CardInfo.CardArtsId).Completed += (obj) =>
        {
            CardImg.sprite = obj.Result;
        };

    }
    private void SetFaction(Image FactionIcon,GwentCard CardInfo)
    {
        if (CardInfo.Group == Cynthia.Card.Group.Gold || CardInfo.Group == Cynthia.Card.Group.Leader)
        {
            if (CardInfo.Faction == Faction.Monsters)
                FactionIcon.sprite = MonstersGoldIcon;
            if (CardInfo.Faction == Faction.Nilfgaard)
                FactionIcon.sprite = NilfgaardGoldIcon;
            if (CardInfo.Faction == Faction.NorthernRealms)
                FactionIcon.sprite = NorthernRealmsGoldIcon;
            if (CardInfo.Faction == Faction.ScoiaTael)
                FactionIcon.sprite = ScoiaTaelGoldIcon;
            if (CardInfo.Faction == Faction.Skellige)
                FactionIcon.sprite = SkelligeGoldIcon;
            if (CardInfo.Faction == Faction.Neutral)
                FactionIcon.sprite = NeutralGoldIcon;
        }
        else
        {
            if (CardInfo.Faction == Faction.Monsters)
                FactionIcon.sprite = MonstersNormalIcon;
            if (CardInfo.Faction == Faction.Nilfgaard)
                FactionIcon.sprite = NilfgaardNormalIcon;
            if (CardInfo.Faction == Faction.NorthernRealms)
                FactionIcon.sprite = NorthernRealmsNormalIcon;
            if (CardInfo.Faction == Faction.ScoiaTael)
                FactionIcon.sprite = ScoiaTaelNormalIcon;
            if (CardInfo.Faction == Faction.Skellige)
                FactionIcon.sprite = SkelligeNormalIcon;
            if (CardInfo.Faction == Faction.Neutral)
                FactionIcon.sprite = NeutralNormalIcon;
        }
    }
    private void SetBorder(Image CardBorder, GwentCard CardInfo)
    {
        if (CardInfo.Group == Cynthia.Card.Group.Gold || CardInfo.Group == Cynthia.Card.Group.Leader)
            CardBorder.sprite = GoldBorder;
        if (CardInfo.Group == Cynthia.Card.Group.Silver)
            CardBorder.sprite = SilverBorder;
        if (CardInfo.Group == Cynthia.Card.Group.Copper)
            CardBorder.sprite = CopperBorder;
    }
}