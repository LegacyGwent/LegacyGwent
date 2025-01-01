using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using MatchInfo;
public class displayflag : MonoBehaviour
{

    public GameObject IsCasualQueue;
    public GameObject IsRankQueue;
    public GameObject IsInMatchRankQueue;
    public GameObject IsInMatchCasualQueue;
    bool iscasualflag = false;
    bool isrankflag = false;
    bool isinmatchcasualflag = false;
    bool isinmatchrankflag = false;
    private void Awake ()
    {
        UpdateFlag();
    }
    public bool IsCasualFlag
    {
        get {return iscasualflag;}
        set {
            iscasualflag = value;
            UpdateFlag();
            }
    }
        public bool IsRankFlag
    {
        get {return isrankflag;}
        set {
            isrankflag = value;
            UpdateFlag();
            }
    }
        public bool IsInMatchCasualFlag
    {
        get {return isinmatchcasualflag;}
        set {
            isinmatchcasualflag = value;
            UpdateFlag();
            }
    }
        public bool IsInMatchRankFlag
    {
        get {return isinmatchrankflag;}
        set {
            isinmatchrankflag = value;
            UpdateFlag();
            }
    }
    void UpdateFlag()
    {
    //Trigger
        if (iscasualflag) {
            IsCasualQueue.SetActive(true);
            IsRankQueue.SetActive(false);
            IsInMatchCasualQueue.SetActive(true);
            IsInMatchRankQueue.SetActive(false);
            }
        else if (isrankflag){
            IsCasualQueue.SetActive(false);
            IsRankQueue.SetActive(true);
            IsInMatchRankQueue.SetActive(true);
            IsInMatchCasualQueue.SetActive(false);
            }
        else
            {IsCasualQueue.SetActive(false);
            IsInMatchCasualQueue.SetActive(false);
            IsRankQueue.SetActive(false);
            IsInMatchRankQueue.SetActive(false);}
    }
}
