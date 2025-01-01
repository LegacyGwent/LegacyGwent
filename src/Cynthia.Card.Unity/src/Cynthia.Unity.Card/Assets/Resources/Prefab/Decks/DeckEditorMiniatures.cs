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
        var op = Addressables.LoadAssetAsync<Sprite>(artid + "_slot");
        Sprite go = op.WaitForCompletion();
        Miniature.sprite = go;
    }
}
