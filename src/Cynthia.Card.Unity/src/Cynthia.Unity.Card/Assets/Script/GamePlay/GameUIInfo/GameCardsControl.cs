using System.Collections;
using System.Collections.Generic;
using Cynthia.Card.Client;
using Cynthia.Card;
using UnityEngine;
using System.Linq;
using Alsein.Extensions;
using System;

public class GameCardsControl : MonoBehaviour
{
    public GameObject MyDeckBack;
    public GameObject EnemyDeckBack;
    //
    public CardsPosition MyHand;
    public CardsPosition MyRow1;
    public CardsPosition MyRow2;
    public CardsPosition MyRow3;
    public LeaderCard MyLeader;
    //
    public CardsPosition EnemyHand;
    public CardsPosition EnemyRow1;
    public CardsPosition EnemyRow2;
    public CardsPosition EnemyRow3;
    public LeaderCard EnemyLeader;
    public CardsPosition StayCards;
    //卡背
    public Sprite NorthernRealmsBack;//北方
    public Sprite ScoiaTaelBack;//松鼠党
    public Sprite MonstersBack;//怪物
    public Sprite SkelligeBack;//群岛
    public Sprite NilfgaardBack;//帝国
    //暂时用不到
    //public CardsPosition MyCemtery;
    //public CardsPosition MyDeck;
    //public CardsPosition EnemyCemtery;
    //public CardsPosition EnemyDeck;
    //预制
    public GameObject CardObj;
    //---------------------------
    public void SetCardsInfo(GameInfomation gameInfomation)
    {
        MyHand.SetCards(gameInfomation.MyHandCard);
        MyRow1.SetCards(gameInfomation.MyPlace[0]);
        MyRow2.SetCards(gameInfomation.MyPlace[1]);
        MyRow3.SetCards(gameInfomation.MyPlace[2]);
        EnemyHand.SetCards(gameInfomation.EnemyHandCard);
        EnemyRow1.SetCards(gameInfomation.EnemyPlace[0]);
        EnemyRow2.SetCards(gameInfomation.EnemyPlace[1]);
        EnemyRow3.SetCards(gameInfomation.EnemyPlace[2]);
        MyLeader.SetLeader(gameInfomation.MyLeader, gameInfomation.IsMyLeader);
        EnemyLeader.SetLeader(gameInfomation.EnemyLeader, gameInfomation.IsEnemyLeader);
        //
        if (gameInfomation.MyLeader.DeckFaction == Faction.Monsters)
            MyDeckBack.GetComponent<SpriteRenderer>().sprite = MonstersBack;
        if (gameInfomation.MyLeader.DeckFaction == Faction.Nilfgaard)
            MyDeckBack.GetComponent<SpriteRenderer>().sprite = NilfgaardBack;
        if (gameInfomation.MyLeader.DeckFaction == Faction.ScoiaTael)
            MyDeckBack.GetComponent<SpriteRenderer>().sprite = ScoiaTaelBack;
        if (gameInfomation.MyLeader.DeckFaction == Faction.Skellige)
            MyDeckBack.GetComponent<SpriteRenderer>().sprite = SkelligeBack;
        if (gameInfomation.MyLeader.DeckFaction == Faction.NorthernRealms)
            MyDeckBack.GetComponent<SpriteRenderer>().sprite = NorthernRealmsBack;
        //
        if (gameInfomation.EnemyLeader.DeckFaction == Faction.Monsters)
            EnemyDeckBack.GetComponent<SpriteRenderer>().sprite = MonstersBack;
        if (gameInfomation.EnemyLeader.DeckFaction == Faction.Nilfgaard)
            EnemyDeckBack.GetComponent<SpriteRenderer>().sprite = NilfgaardBack;
        if (gameInfomation.EnemyLeader.DeckFaction == Faction.ScoiaTael)
            EnemyDeckBack.GetComponent<SpriteRenderer>().sprite = ScoiaTaelBack;
        if (gameInfomation.EnemyLeader.DeckFaction == Faction.Skellige)
            EnemyDeckBack.GetComponent<SpriteRenderer>().sprite = SkelligeBack;
        if (gameInfomation.EnemyLeader.DeckFaction == Faction.NorthernRealms)
            EnemyDeckBack.GetComponent<SpriteRenderer>().sprite = NorthernRealmsBack;
    }
}
