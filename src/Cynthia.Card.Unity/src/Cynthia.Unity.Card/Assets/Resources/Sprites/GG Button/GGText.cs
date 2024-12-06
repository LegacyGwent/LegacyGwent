using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GGText : MonoBehaviour
{
    [SerializeField] Text OpponentText;        
    
    string opponent = "";

    private void Awake ()
    {
        UpdateHUD();
    }
    public string Opponent
    {
        get {return opponent;}
        set {
            opponent = value;
            UpdateHUD();
            }
    }
    
    private void UpdateHUD()
    {
        OpponentText.text = opponent.ToString ();
    }
    

}
