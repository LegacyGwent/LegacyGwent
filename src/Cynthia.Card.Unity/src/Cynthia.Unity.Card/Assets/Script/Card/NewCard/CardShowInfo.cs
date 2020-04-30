using System.Collections;
using System.Collections.Generic;
using Cynthia.Card;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class CardShowInfo : MonoBehaviour
{
    public CardMoveInfo CardMoveInfo;
    //?
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public bool _isDead = false;
    public CardStatus CurrentCore
    {
        get => _currentCore; set
        {
            if (_currentCore != null && (_currentCore.IsCardBack != value.IsCardBack))
            {
                _currentCore = value;
                Reverse();//反转
                return;
            }
            _currentCore = value;
            //_currentCore.CardInfo = GwentMap.CardMap[_currentCore.CardId];
            SetCard();
        }
    }                       //卡id
    public GwentCard CardInfo { get => GwentMap.CardMap[_currentCore.CardId]; }   //卡类型
    public bool IsGray
    {
        get => _isGray;
        set
        {
            if (_isGray == value)
                return;
            _isGray = value;
            CardStatus.SetActive(IsGray);
        }
    }
    private bool _isGray = false;
    private CardStatus _currentCore;
    //---------------------------
    public Text Strength;
    public Text Armor;
    public Text Countdown;
    public GameObject StrengthShow;
    public GameObject ArmorShow;
    public GameObject CountdownShow;
    //---------------------------
    public Image FactionIcon;
    public Image CardBorder;
    public Image CardImg;
    public Image CardBack;
    public GameObject CardStatus;
    public GameObject LockIcon;//锁定
    public GameObject SpyingIcon;//间谍
    public GameObject Resilience;//坚韧
    public GameObject RevealIcon;//揭示
    //-----------------------------
    public Sprite CopperBorder;
    public Sprite SilverBorder;
    public Sprite GoldBorder;
    //--------
    public Sprite NorthernRealmsIcon;//北方
    public Sprite ScoiaTaelIcon;//松鼠党
    public Sprite MonstersIcon;//怪物
    public Sprite SkelligeIcon;//群岛
    public Sprite NilfgaardIcon;//帝国
    public Sprite NeutralIcon;//中立
    //--------
    public Sprite NorthernRealmsBack;//北方
    public Sprite ScoiaTaelBack;//松鼠党
    public Sprite MonstersBack;//怪物
    public Sprite SkelligeBack;//群岛
    public Sprite NilfgaardBack;//帝国
    //----------------------------------
    //客户端相关
    public Image SelectCenter;
    public Image SelectMargin;
    public GameObject SelectIcon;

    //目前被CurrentCore属性取代
    //public CardShowInfo(CardStatus card) => CurrentCore = card;

    //设定选取状态
    public void SetSelect(bool center, bool margin, bool isLight = false)
    {
        SelectCenter.gameObject.SetActive(center);
        SelectMargin.gameObject.SetActive(margin);
        SelectIcon.SetActive(center || margin);
        if (isLight)
        {
            SelectCenter.color = new Color(0, 160f / 255f, 1);
            SelectMargin.color = new Color(0, 160f / 255f, 1);
        }
        else
        {
            SelectCenter.color = new Color(0, 180f / 255f, 1);
            SelectMargin.color = new Color(0, 180f / 255f, 1);
        }
    }

    //根据CurrentCore来刷新卡面
    public void SetCard()
    {
        // Debug.Log("刷新了卡牌设置");
        // Debug.Log($"卡牌名称是:{CurrentCore.Name},生命状态是:{CurrentCore.HealthStatus}");
        var iconCount = 0;
        var use = this.GetComponent<CardMoveInfo>();
        if (use != null && !CurrentCore.IsCardBack)
            use.CardUseInfo = CardInfo.CardUseInfo;
        CardImg.sprite = Resources.Load<Sprite>("Sprites/Cards/" + CurrentCore.CardArtsId);
        //设置卡牌是否灰(转移到属性)
        //如果卡牌是背面,设置背面并结束
        CardBack.gameObject.SetActive(false);
        if (CurrentCore.IsCardBack)
        {
            RevealIcon.SetActive(false);
            LockIcon.SetActive(false);
            Resilience.SetActive(false);
            SpyingIcon.SetActive(false);
            if (CurrentCore.DeckFaction == Faction.Monsters)
                CardBack.sprite = MonstersBack;
            if (CurrentCore.DeckFaction == Faction.Nilfgaard)
                CardBack.sprite = NilfgaardBack;
            if (CurrentCore.DeckFaction == Faction.NorthernRealms)
                CardBack.sprite = NorthernRealmsBack;
            if (CurrentCore.DeckFaction == Faction.ScoiaTael)
                CardBack.sprite = ScoiaTaelBack;
            if (CurrentCore.DeckFaction == Faction.Skellige)
                CardBack.sprite = SkelligeBack;
            CardBack.gameObject.SetActive(true);
            return;
        }
        if (CurrentCore.Group == Group.Gold || CurrentCore.Group == Group.Leader)
            CardBorder.sprite = GoldBorder;
        if (CurrentCore.Group == Group.Silver)
            CardBorder.sprite = SilverBorder;
        if (CurrentCore.Group == Group.Copper)
            CardBorder.sprite = CopperBorder;
        if (CardInfo.Faction == Faction.Monsters)
            FactionIcon.sprite = MonstersIcon;
        if (CardInfo.Faction == Faction.Nilfgaard)
            FactionIcon.sprite = NilfgaardIcon;
        if (CardInfo.Faction == Faction.NorthernRealms)
            FactionIcon.sprite = NorthernRealmsIcon;
        if (CardInfo.Faction == Faction.ScoiaTael)
            FactionIcon.sprite = ScoiaTaelIcon;
        if (CardInfo.Faction == Faction.Skellige)
            FactionIcon.sprite = SkelligeIcon;
        if (CardInfo.Faction == Faction.Neutral)
            FactionIcon.sprite = NeutralIcon;
        CountdownShow.SetActive(CurrentCore.IsCountdown);
        if (CurrentCore.IsCountdown)
        {
            Countdown.text = CurrentCore.Countdown.ToString();
            iconCount++;
        }
        //揭示
        RevealIcon.SetActive(CurrentCore.IsReveal);
        if (CardInfo.CardType == CardType.Special)
        {
            Strength.gameObject.SetActive(false);
            FactionIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 100);
            return;
        }
        Strength.gameObject.SetActive(true);
        iconCount++;
        //根据状态进行设置
        Armor.text = CurrentCore.Armor.ToString();
        if (CurrentCore.Armor > 0)
        {
            ArmorShow.SetActive(true);
            iconCount++;
        }
        else
            ArmorShow.SetActive(false);
        LockIcon.SetActive(CurrentCore.IsLock);
        Resilience.SetActive(CurrentCore.IsResilience);
        SpyingIcon.SetActive(CurrentCore.IsSpying);
        Strength.text = (CurrentCore.Strength + CurrentCore.HealthStatus).ToString();
        if (CurrentCore.HealthStatus > 0)
            Strength.color = ClientGlobalInfo.GreenColor;
        if (CurrentCore.HealthStatus == 0)
            Strength.color = ClientGlobalInfo.NormalColor;
        if (CurrentCore.HealthStatus < 0)
            Strength.color = ClientGlobalInfo.RedColor;
        FactionIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50 + (iconCount == 0 ? 1 : iconCount) * 50);
        //-----------------------------------------------
    }

    //-------------------------------------------------------------------------------------------
    //DOTween动画
    public void ShowCardBreak(CardBreakEffectType type)
    {
        IsDead = true;
        //Debug.Log(IsDead);
        //GetComponent<Animator>().Play("DemoBreak");
        //CardMoveInfo.Destroy();
        DOTween.Sequence().Append(transform.DOScale(0, 0.5f))
            .AppendCallback(CardMoveInfo.Destroy);
    }
    public void Reverse()
    {
        DOTween.Sequence().Append(transform.DOLocalRotate(new Vector3(0, 90, 0), 0.15f))
            .AppendCallback(SetCard)
            .Append(transform.DOLocalRotate(new Vector3(0, 0, 0), 0.15f));
        if(AmbushType() && !_currentCore.IsCardBack)
        {
            if(CurrentCore.CardArtsId == "11221000")//Roach卡
            {
                AudioManager.Instance.PlayAudio(CurrentCore.CardArtsId, AudioType.Card, AudioPlayMode.PlayOneShoot);
            }
            else
               AudioManager.Instance.PlayAudio(CurrentCore.CardArtsId, AudioType.Card, AudioPlayMode.Append);
        }
    }



    public void PlayAudio()
    {
        if (CurrentCore == null)
            return;
        if (CardInfo.CardType == CardType.Unit && !AmbushType())
        {
            Debug.Log("播放Unit卡片声音" + CurrentCore.CardArtsId);
            if (CurrentCore.CardArtsId == "11221000") //Roach卡
            {
                AudioManager.Instance.PlayAudio(CurrentCore.CardArtsId, AudioType.Card, AudioPlayMode.PlayOneShoot);
            }
            else
                AudioManager.Instance.PlayAudio(CurrentCore.CardArtsId, AudioType.Card, AudioPlayMode.Append);
        }
        else if (CardInfo.CardType == CardType.Special && !SpecialCardID.isSpecialCard(CurrentCore.CardArtsId))
        {
            Debug.Log("播放特殊卡片声音" + CurrentCore.CardArtsId);
            AudioManager.Instance.PlayAudio(CurrentCore.CardArtsId, AudioType.Card, AudioPlayMode.Append);
        }
    }

    public void SpecialAudioPlay()
    {
        if (CurrentCore == null)
            return;
        if (CardInfo.CardType == CardType.Special && SpecialCardID.isSpecialCard(CurrentCore.CardArtsId))
        {
            Debug.Log("播放特殊特殊卡片声音" + CurrentCore.CardArtsId);
            AudioManager.Instance.PlayAudio(CurrentCore.CardArtsId, AudioType.Card, AudioPlayMode.Append);
        }
    }

    private bool AmbushType()
    {
        if (CurrentCore == null)
            return true;
        if (CurrentCore.Categories == null)
            return false;
        for (int i = 0; i < CurrentCore.Categories.Length; i++)
        {
            if(CurrentCore.Categories[i] == Categorie.Ambush)
            {
                return true;
            }
        }
        return false;
    }

    public void ScaleTo(float endValue, float duration = 0.25f, Ease ease = Ease.OutQuad)
    {
        transform.DOScale(endValue, duration).SetEase(ease);
    }
}
