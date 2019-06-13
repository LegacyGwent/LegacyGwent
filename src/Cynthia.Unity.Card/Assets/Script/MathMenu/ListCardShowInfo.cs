using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card;

public class ListCardShowInfo : MonoBehaviour
{
    public Text Strength;
    public Text Name;
    public GameObject Count;
    public Image Star;
    public Text CountText;
    public Image Border;
    public Sprite Copper;
    public Sprite Silver;
    public Sprite Gold;
    public Sprite CopperStar;
    public Sprite SilverStar;
    public Sprite GoldStar;
    public void SetCardInfo(int strength,string name,int count = 1,Group group = Group.Gold)
    {
        Border.sprite = (group == Group.Gold ? Gold : (group==Group.Silver?Silver:Copper));
        Strength.text = strength.ToString();
        Name.text = name;
        if (strength <= 0)
        {
            Star.gameObject.SetActive(true);
            Strength.gameObject.SetActive(false);
            Star.sprite = (group == Group.Gold ? GoldStar : (group == Group.Silver ? SilverStar : CopperStar));
        }
        if (count > 1)
        {
            Count.SetActive(true);
            CountText.text = $"x{count.ToString()}";
        }
    }
    public void SetCardInfo(CardStatus card,int count = 1)
    {
        SetCardInfo(card.Strength, card.CardInfo().Name, 1, card.Group);
    }
    public void SetCardInfo(string id,int count = 1)
    {
        var card = GwentMap.CardMap[id];
        SetCardInfo(card.Strength,card.Name,count,card.Group);
    }
}
