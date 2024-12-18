using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card;
using UnityEngine.AddressableAssets;

public class DeckEditorMiniatures : MonoBehaviour
{
    public Image DeckLeaderArt;
    // public CardStatus CurrentCore
    // {
    //     get => _currentCore; set
    //     {
    //         setCurrentCore(value);
    //     }
    // }                       //卡id
    // private CardStatus _currentCore;
    // public GwentCard CardInfo { get => GwentMap.CardMap[_currentCore.CardId]; }   //卡类型
    // private string _oldCardArtsId = null; // prevent repeated load
    // void Start()
    // {
        
    // }
    // public void SetDeckMiniature()
    // {
        
    // }
    // public void setCurrentCore(CardStatus value, bool asyncLoadAsset = false)
    // {
    //     if (_currentCore != null)
    //     {
    //         _currentCore = value;
    //         return;
    //     }
    //     _currentCore = value;
    //     SetLeaderArt(asyncLoadAsset);
    // }
    public void SetLeaderArt(string artid)
    {
        // Debug.Log("刷新了卡牌设置");
        // Debug.Log($"卡牌名称是:{CurrentCore.Name},生命状态是:{CurrentCore.HealthStatus}");
        // var use = this.GetComponent<CardMoveInfo>();
        // if (use != null)
        //     use.CardUseInfo = CardInfo.CardUseInfo;
        // if (CurrentCore.CardArtsId != null && _oldCardArtsId != CurrentCore.CardArtsId)
        // {
        //     if (asyncLoadAsset)
        //     {
        //         Addressables.LoadAssetAsync<Sprite>(CurrentCore.CardArtsId+"_slot").Completed += (obj) =>
        //         {
        //             DeckLeaderArt.sprite = obj.Result;
        //         };
        //     }
        //     else
        //     {
        //         DeckLeaderArt.sprite = Addressables.LoadAssetAsync<Sprite>(CurrentCore.CardArtsId+"_slot").WaitForCompletion();
        //     }
        //     _oldCardArtsId = CurrentCore.CardArtsId;
        // }
        Addressables.LoadAssetAsync<Sprite>(artid + "_slot").Completed += (obj) =>
        {
            DeckLeaderArt.sprite = obj.Result;
        };
    }
}
