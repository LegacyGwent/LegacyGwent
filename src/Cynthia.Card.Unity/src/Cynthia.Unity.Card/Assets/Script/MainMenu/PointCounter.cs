using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Assets.Script.Localization;



public class PointCounter : MonoBehaviour
{
    
    [SerializeField] counterHUD counterHUD;
    public Text PlayerCounttNum;
    //-----------------------------------
    private LocalizationService _translator;
    private GwentClientService server;
    
    
    private void Start ()
    {
        int timer = 0;
        StartCoroutine (CountPoints() );
    }
    private IEnumerator CountPoints()
    {
        while (true)
        {
            counterHUD.Points = 2;
        yield return new WaitForSeconds(2);
        }
    }
}