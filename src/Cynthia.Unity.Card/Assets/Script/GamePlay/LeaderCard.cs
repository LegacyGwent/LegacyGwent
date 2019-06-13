using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card;
using System.Linq;
using Alsein.Extensions;

public class LeaderCard : MonoBehaviour
{
    public RowPosition Id;
    public bool IsCardCanUse;
    public GameObject CardPrefab;
    public bool IsCanSelect;
    public bool IsCanDrag;
    public GameObject TrueCard;
    public GameObject GrayCard;
    public void SetLeader(CardStatus Leader,bool isCardCanUse)
    {
        IsCardCanUse = isCardCanUse;
        DestroyAllChild();
        if (isCardCanUse)
        {
            var newCard = Instantiate(CardPrefab);
            newCard.GetComponent<CardShowInfo>().CurrentCore = Leader;//设定卡牌信息
            newCard.GetComponent<CardMoveInfo>().CardUseInfo = GwentMap.CardMap[Leader.CardId].CardUseInfo;//设置卡牌信息
            //newCard.GetComponent<CardShowInfo>().SetCard();//展现卡牌样式
            newCard.transform.SetParent(transform);//设置卡牌的父物体
            newCard.transform.localPosition = new Vector3(0, 0, -0.01f);//设置卡牌的坐标
            newCard.transform.localScale = Vector3.one;//设置缩放
            newCard.GetComponent<CardMoveInfo>().SetResetPoint(new Vector3(0, 0, -0.01f));//设置坐标
            newCard.GetComponent<CardMoveInfo>().IsCanDrag = IsCanDrag;//设置是否可拖
            newCard.GetComponent<CardMoveInfo>().IsCanSelect = IsCanSelect;//是否可选
            TrueCard = newCard;
        }
        //--------------------------------
        var grayCard = Instantiate(CardPrefab);
        grayCard.GetComponent<CardShowInfo>().CurrentCore = Leader;//设定卡牌信息
        grayCard.GetComponent<CardShowInfo>().IsGray = true;
        grayCard.GetComponent<CardMoveInfo>().CardUseInfo = GwentMap.CardMap[Leader.CardId].CardUseInfo;//设置卡牌信息
        //grayCard.GetComponent<CardShowInfo>().SetCard();//展现卡牌样式
        grayCard.transform.SetParent(transform);//设置卡牌的父物体
        grayCard.transform.localPosition = new Vector3(0, 0, 0);//设置卡牌的坐标
        grayCard.transform.localScale = Vector3.one;//设置缩放
        grayCard.GetComponent<CardMoveInfo>().SetResetPoint(new Vector3(0, 0, 0));//设置坐标
        grayCard.GetComponent<CardMoveInfo>().IsCanDrag = false;//设置是否可拖
        grayCard.GetComponent<CardMoveInfo>().IsCanSelect = false;//是否可选
        GrayCard = grayCard;
        if(isCardCanUse)
            grayCard.SetActive(false);
    }
    public void AutoSet()
    {
        if (TrueCard == null && GrayCard != null)
            GrayCard.SetActive(true);
    }
    public void SetCanDrag(bool isCanDrag)
    {
        IsCanDrag = isCanDrag;
        if (TrueCard != null)
            TrueCard.GetComponent<CardMoveInfo>().IsCanDrag = IsCanDrag;
        else
            IsCanDrag = false;
    }
    public void SetCanSelect(bool isCanSelect)
    {
        IsCanSelect = isCanSelect;
        if (TrueCard != null)
            TrueCard.GetComponent<CardMoveInfo>().IsCanSelect = isCanSelect;
        else
            IsCanSelect = false;
    }
    private void DestroyAllChild()
    {
        var count = transform.childCount;
        for(int i = count; i > 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
