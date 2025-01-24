using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.Localization;
using Autofac;
using UnityEngine.AddressableAssets;
using Cynthia.Card;

public class LinkedCard : MonoBehaviour
{
    //logic
    public GwentCard CardInfo { get; private set; }
    public string ID;
    public righclickLogic myRightClickLogic;
    private LocalizationService translator;
    //fields
    public Text Strength;
    public Text Countdown;
    public GameObject CountdownShow;
    public Image FactionIcon;
    public Image CardBorder;
    public Image CardImg;
    public Text  NameDisplay;
    public Text TagsDisplay;
    public Text  AbilityDisplay;
    //sprites
    public Sprite CopperBorder;
    public Sprite SilverBorder;
    public Sprite GoldBorder;
    public Sprite NorthernRealmsBasicIcon;
    public Sprite ScoiaTaelBasicIcon;
    public Sprite MonstersBasicIcon;
    public Sprite SkelligeBasicIcon;
    public Sprite NilfgaardBasicIcon;
    public Sprite NeutralBasicIcon;

    public void UpdateOnButtonClick()
    {
        myRightClickLogic.UpdateCard(ID);
    }
    public void UpdateSelfContent()
    {
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
        CardInfo = GwentMap.CardMap[ID];

        if (CardInfo.Group == Group.Gold || CardInfo.Group == Group.Leader)
            CardBorder.sprite = GoldBorder;
        if (CardInfo.Group == Group.Silver)
            CardBorder.sprite = SilverBorder;
        if (CardInfo.Group == Group.Copper)
            CardBorder.sprite = CopperBorder;
        
        Addressables.LoadAssetAsync<Sprite>(CardInfo.CardArtsId).Completed += (obj) =>
        {
            CardImg.sprite = obj.Result;
        };

        
        if (CardInfo.Faction == Faction.Monsters)
            FactionIcon.sprite = MonstersBasicIcon;
        if (CardInfo.Faction == Faction.Nilfgaard)
            FactionIcon.sprite = NilfgaardBasicIcon;
        if (CardInfo.Faction == Faction.NorthernRealms)
            FactionIcon.sprite = NorthernRealmsBasicIcon;
        if (CardInfo.Faction == Faction.ScoiaTael)
            FactionIcon.sprite = ScoiaTaelBasicIcon;
        if (CardInfo.Faction == Faction.Skellige)
            FactionIcon.sprite = SkelligeBasicIcon;
        if (CardInfo.Faction == Faction.Neutral)
            FactionIcon.sprite = NeutralBasicIcon;
        
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
        //
        RectTransform rectTransform = FactionIcon.GetComponent<RectTransform>();
        Vector2 currentSize = rectTransform.sizeDelta;
        if(CardInfo.IsCountdown)
        {
            rectTransform.sizeDelta = new Vector2(currentSize.x, 180);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(currentSize.x, 140);
        }
        //
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
        AbilityDisplay.text = righclickLogic.RemoveContentInParentheses(translator.GetCardInfo(CardInfo.CardId));

    }

}
