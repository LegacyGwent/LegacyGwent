using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class counterHUD : MonoBehaviour
{
    [SerializeField] Text usersText;
    [SerializeField] Text usersinmatchText;
    [SerializeField] Text usersvsaiText;
    [SerializeField] Text usersinrankedText;
    [SerializeField] Text usersincasualText;    
    
    int points = 0;
    int usersinmatch = 0;
    int usersvsai = 0;
    int usersinranked = 0;
    int usersincasual = 0;
    private void Awake ()
    {
        UpdateHUD();
    }
    public int Users
    {
        get {return points;}
        set {
            points = value;
            UpdateHUD();
            }
    }
    public int UsersInMatch
    {
        get {return usersinmatch;}
        set {
            usersinmatch = value;
            UpdateHUD();
            }
    }
    public int UsersvsAI
    {
        get {return usersvsai;}
        set {
            usersvsai = value;
            UpdateHUD();
            }
    }
    public int UsersInRanked
    {
        get {return usersinranked;}
        set {
            usersinranked = value;
            UpdateHUD();
            }
    }
    public int UsersInCasual
    {
        get {return usersincasual;}
        set {
            usersincasual = value;
            UpdateHUD();
            }
    }
    private void UpdateHUD()
    {
        usersText.text = points.ToString ();
        usersinmatchText.text = usersinmatch.ToString ();
        usersvsaiText.text = usersvsai.ToString();
        usersinrankedText.text = usersinranked.ToString();
        usersincasualText.text = usersincasual.ToString();
    }
    

}
