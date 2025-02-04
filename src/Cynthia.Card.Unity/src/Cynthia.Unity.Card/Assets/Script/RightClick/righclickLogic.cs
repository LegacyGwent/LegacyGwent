﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.Localization;
using Autofac;
using UnityEngine.AddressableAssets;
using Cynthia.Card;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
//using TMPro;

public class righclickLogic : MonoBehaviour
{
    //logic
    public GwentCard CardInfo { get; private set; }
    private LocalizationService translator;
    public List<string> History;
    public string DisplayID;
    //fields
    public GameObject LinkedCard;
    public Transform ScrollContent;
    public Text ExitButtonText;
    public Text BackButtonText;
    public GameObject Slider;
    public RectTransform layoutGroup;
    public Text Strength;
    public Text Countdown;
    public GameObject CountdownShow;
    public Image FactionIcon;
    public Image CardBorder;
    public Image CardImg;
    public Text NameDisplay;
    public Text TagsDisplay;
    public Text FlavourDisplay;
    public Text AbilityDisplay;
    
    
    //sprites
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
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
        ExitButtonText.text=translator.GetText("LoginMenu_ExitButton");
        BackButtonText.text=translator.GetText("RegisterMenu_BackButton");
        History = new List<string>();
        string DisplayID = GameEvent.RightClickedCardID;
        UpdateCard(DisplayID);
    }
    public static string RemoveContentInParentheses(string input)
    {
        // Use Regex to find and remove content within parentheses
        return System.Text.RegularExpressions.Regex.Replace(input, @"\s*\([^)]*\)", "").Trim();
    }
    public void UpdateCard(string CardId)
    {
        DisplayID=CardId;
        History.Add(DisplayID);
        CardInfo = GwentMap.CardMap[CardId];
        if (CardInfo.Group == Cynthia.Card.Group.Gold || CardInfo.Group == Cynthia.Card.Group.Leader)
            CardBorder.sprite = GoldBorder;
        if (CardInfo.Group == Cynthia.Card.Group.Silver)
            CardBorder.sprite = SilverBorder;
        if (CardInfo.Group == Cynthia.Card.Group.Copper)
            CardBorder.sprite = CopperBorder;

        
        Addressables.LoadAssetAsync<Sprite>(CardInfo.CardArtsId).Completed += (obj) =>
        {
            CardImg.sprite = obj.Result;
        };


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
        
        if (CardInfo.CardType == CardType.Special)
        {
            Strength.gameObject.SetActive(false);
        }
        else
        {
            Strength.gameObject.SetActive(true);
        }
        Strength.text = (CardInfo.Strength).ToString();

        CountdownShow.SetActive(CardInfo.IsCountdown);
        Countdown.text = CardInfo.Countdown.ToString();

        NameDisplay.text=translator.GetCardName(CardInfo.CardId);

        string tagtext="";
        if (CardInfo.Categories.Length > 0)
        {
            foreach (Categorie categorie in CardInfo.Categories)
            {
                tagtext=tagtext+translator.GetText($"CardTag_"+categorie)+", ";
            }
            tagtext = tagtext.Remove(tagtext.Length - 2);
        }
        TagsDisplay.text=tagtext;

        AbilityDisplay.text=RemoveContentInParentheses(translator.GetCardInfo(CardInfo.CardId));
        FlavourDisplay.text=translator.GetCardFlavor(CardInfo.CardId);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);

        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }
        List<string> LinkedCardsList = CardInfo.LinkedCards;
        //Debug.Log(CardInfo.LinkedCards);
        if (LinkedCardsList.Count > 3)
        {
            Slider.SetActive(true);
        }
        else
        {
            Slider.SetActive(false);
        }

        foreach (string ID in LinkedCardsList)
        {
            AddLinked(ID);
        }
    }
    public void AddLinked(string ID)
    {
        GameObject instance = Instantiate(LinkedCard, ScrollContent.position, ScrollContent.rotation, ScrollContent);
        LinkedCard linkedCardScript = instance.GetComponent<LinkedCard>();
        linkedCardScript.myRightClickLogic = this;
        linkedCardScript.ID = ID;
        linkedCardScript.UpdateSelfContent(); 
    }
    public void Closerightclick()
    {
        GameEvent.RighClickActive=false;
        SceneManager.UnloadSceneAsync("RightClick");
    }
    public void BackButton()
    {
        History.RemoveAt(History.Count - 1);
        if (History.Count > 0) // Check if the list is not empty
        {
            DisplayID=History[History.Count - 1];
            UpdateCard(DisplayID);
            History.RemoveAt(History.Count - 1);
        }
        else
        {
            Closerightclick();
        }
    }
}
