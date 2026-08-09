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
        // Deck headers use several frame widths; fill the painted slot instead
        // of letterboxing it and exposing the frame background at the edges.
        Miniature.preserveAspect = false;
        LeaderSlotClip.Apply(Miniature);
    }
}
