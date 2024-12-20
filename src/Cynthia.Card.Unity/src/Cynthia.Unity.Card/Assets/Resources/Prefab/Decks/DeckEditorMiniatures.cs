using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card;
using UnityEngine.AddressableAssets;

public class DeckEditorMiniatures : MonoBehaviour
{
    public Image Miniature;
    public void SetMiniatureArt(string artid)
    {
        Addressables.LoadAssetAsync<Sprite>(artid + "_slot").Completed += (obj) =>
        {
            Miniature.sprite = obj.Result;
        };
    }
}
