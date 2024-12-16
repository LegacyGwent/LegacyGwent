using Cynthia.Card;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class ArtCard : MonoBehaviour
{
    public CardStatus CurrentCore
    {
        get => _currentCore; set
        {
            _currentCore = value;
            Content.SetCard(_currentCore);
            SetCard();
        }
    }
    public GwentCard CardInfo { get => GwentMap.CardMap[_currentCore.CardId]; }
    public CardContent Content;
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
    //-----------------------------
    public Sprite CopperBorder;
    public Sprite SilverBorder;
    public Sprite GoldBorder;
    //--------
    public Sprite NorthernRealmsNormalIcon;//北方
    public Sprite ScoiaTaelNormalIcon;//松鼠党
    public Sprite MonstersNormalIcon;//怪物
    public Sprite SkelligeNormalIcon;//群岛
    public Sprite NilfgaardNormalIcon;//帝国
    public Sprite NeutralNormalIcon;//中立
    //--------
    public Sprite NorthernRealmsGoldIcon;//北方
    public Sprite ScoiaTaelGoldIcon;//松鼠党
    public Sprite MonstersGoldIcon;//怪物
    public Sprite SkelligeGoldIcon;//群岛
    public Sprite NilfgaardGoldIcon;//帝国
    public Sprite NeutralGoldIcon;//中立
    //--------
    public Sprite NorthernRealmsBack;//北方
    public Sprite ScoiaTaelBack;//松鼠党
    public Sprite MonstersBack;//怪物
    public Sprite SkelligeBack;//群岛
    public Sprite NilfgaardBack;//帝国
    //----------------------------------

    // public void Start()
    // {
    //     CurrentCore = new CardStatus(Faction.Monsters);
    // }
    //根据CurrentCore来刷新卡面
    public void SetCard()
    {
        Content.gameObject.SetActive(!(_currentCore.IsCardBack || _currentCore.Conceal));
        var iconCount = 0;
        var use = this.GetComponent<CardMoveInfo>();
        if (use != null && !CurrentCore.IsCardBack)
            use.CardUseInfo = CardInfo.CardUseInfo;
        if (CurrentCore.CardArtsId != null)
        {
            Addressables.LoadAssetAsync<Sprite>(CurrentCore.CardArtsId).Completed += (obj) =>
            {
                CardImg.sprite = obj.Result;
            };
            // CardImg.sprite = Addressables.LoadAssetAsync<Sprite>(CurrentCore.CardArtsId).WaitForCompletion();
        }
        // CardImg.sprite = Resources.Load<Sprite>("Sprites/Cards/" + CurrentCore.CardArtsId);
        //设置卡牌是否灰(转移到属性)
        //如果卡牌是背面,设置背面并结束
        CardBack.gameObject.SetActive(false);
        if (CurrentCore.IsCardBack)
        {
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
            FactionIcon.gameObject.SetActive(false);
            return;
        }
        FactionIcon.gameObject.SetActive(true);
        if (CurrentCore.Group == Group.Gold || CurrentCore.Group == Group.Leader)
            CardBorder.sprite = GoldBorder;
        if (CurrentCore.Group == Group.Silver)
            CardBorder.sprite = SilverBorder;
        if (CurrentCore.Group == Group.Copper)
            CardBorder.sprite = CopperBorder;
        if (CardInfo.Group == Group.Gold || CardInfo.Group == Group.Leader)
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
        CountdownShow.SetActive(CurrentCore.IsCountdown);
        if (CurrentCore.IsCountdown)
        {
            Countdown.text = CurrentCore.Countdown.ToString();
            iconCount++;
        }
        if (CardInfo.CardType == CardType.Special)
        {
            StrengthShow.SetActive(false);
            ArmorShow.SetActive(false);
            // FactionIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 100);
            return;
        }
        StrengthShow.SetActive(true);
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
        Strength.text = (CurrentCore.Strength).ToString();
        Strength.color = ClientGlobalInfo.NormalColor;
        // FactionIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50 + (iconCount == 0 ? 1 : iconCount) * 50);
        //-----------------------------------------------
    }

    //-------------------------------------------------------------------------------------------
    //DOTween动画
}
