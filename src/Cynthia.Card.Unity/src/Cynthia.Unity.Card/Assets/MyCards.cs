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
    [Header ("Animations")]
    public Animator animator;
    [Header("Fields")]
    public Text Strength;
    public Image FactionIcon;
    public Image CardBorder;
    public Image CardImg;
    public Image BackCard;

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

    public Sprite NorthernRealmsBack;
    public Sprite ScoiaTaelBack;
    public Sprite MonstersBack;
    public Sprite SkelligeBack;
    public Sprite NilfgaardBack;
    public Sprite NeutralBack;

    void Start()
    {
        animator = GetComponent<Animator>();
        //animator.speed = 0.5f;
        //translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    public void SetCard(string CardId)
    {
        //Debug.Log("xxxxxxx "+CardId);
        
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
            {
                FactionIcon.sprite = MonstersGoldIcon;
                BackCard.sprite = MonstersBack;
            }
            else if (CardInfo.Faction == Faction.Nilfgaard)
            {
                FactionIcon.sprite = NilfgaardGoldIcon;
                BackCard.sprite = NilfgaardBack;
            }
            else if (CardInfo.Faction == Faction.NorthernRealms)
            {
                FactionIcon.sprite = NorthernRealmsGoldIcon;
                BackCard.sprite = NorthernRealmsBack;
            }
            else if (CardInfo.Faction == Faction.ScoiaTael)
            {
                FactionIcon.sprite = ScoiaTaelGoldIcon;
                BackCard.sprite = ScoiaTaelBack;
            }
            else if (CardInfo.Faction == Faction.Skellige)
            {
                FactionIcon.sprite = SkelligeGoldIcon;
                BackCard.sprite = SkelligeBack;
            }
            else if (CardInfo.Faction == Faction.Neutral)
            {
                FactionIcon.sprite = NeutralGoldIcon;
                BackCard.sprite = NeutralBack;
            }
        }
        else
        {
            if (CardInfo.Faction == Faction.Monsters)
            {
                FactionIcon.sprite = MonstersNormalIcon;
                BackCard.sprite = MonstersBack;
            }
            else if (CardInfo.Faction == Faction.Nilfgaard)
            {
                FactionIcon.sprite = NilfgaardNormalIcon;
                BackCard.sprite = NilfgaardBack;
            }
            else if (CardInfo.Faction == Faction.NorthernRealms)
            {
                FactionIcon.sprite = NorthernRealmsNormalIcon;
                BackCard.sprite = NorthernRealmsBack;
            }
            else if (CardInfo.Faction == Faction.ScoiaTael)
            {
                FactionIcon.sprite = ScoiaTaelNormalIcon;
                BackCard.sprite = ScoiaTaelBack;
            }
            else if (CardInfo.Faction == Faction.Skellige)
            {
                FactionIcon.sprite = SkelligeNormalIcon;
                BackCard.sprite = SkelligeBack;
            }
            else if (CardInfo.Faction == Faction.Neutral)
            {
                FactionIcon.sprite = NeutralNormalIcon;
                BackCard.sprite = NeutralBack;
            }
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
    public void MyCardMoveOut()
    {
        animator.Play("MyCardMoveOut", 0, 0f);
    }
    public void EnemyCardMoveOut()
    {
        animator.Play("EnemyCardMoveOut", 0, 0f);
    }
    public void MyCardMoveIn()
    {
        animator.Play("MyCardMoveIn", 0, 0f);
    }
    public void EnemyCardMoveIn()
    {
        animator.Play("EnemyCardMoveIn", 0, 0f);
    }
}