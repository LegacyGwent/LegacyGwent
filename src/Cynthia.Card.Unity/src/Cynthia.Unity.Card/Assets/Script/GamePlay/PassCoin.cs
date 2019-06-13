using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card;

public class PassCoin : MonoBehaviour
{
    public GameObject Coin;
    public GameObject FactionIcon;
    public Faction MyFaction;
    public Faction EnemyFaction;
    public Sprite RedIcon;
    public Sprite BlueIcon;
    public Sprite NorthernRealmsIcon;
    public Sprite ScoiaTaelIcon;
    public Sprite MonstersIcon;
    public Sprite SkelligeIcon;
    public Sprite NilfgaardIcon;
    public bool IsCanUse = false;

    public bool IsMyRound
    {   get => _isMyRound;
        set
        {
            if (value)
                gameObject.GetComponent<Animator>().Play("CoinToBlue");
            else
                gameObject.GetComponent<Animator>().Play("CoinToRed");
            //Coin.GetComponent<SpriteRenderer>().sprite = (value ? BlueIcon : RedIcon);
            _isMyRound = value;
        }
    }
    private bool _isMyRound;

    private void CoinToRed()
    {
        Coin.GetComponent<SpriteRenderer>().sprite = RedIcon;
        gameObject.GetComponent<Animator>().Play("CoinShow");
    }
    private void CoinToBlue()
    {
        Coin.GetComponent<SpriteRenderer>().sprite = BlueIcon;
        gameObject.GetComponent<Animator>().Play("CoinShow");
    }
}
