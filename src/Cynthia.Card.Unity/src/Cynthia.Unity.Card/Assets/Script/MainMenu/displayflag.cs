using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayflag : MonoBehaviour
{
    public GameObject IsCasualQueue;
    public GameObject IsRankQueue;
    bool iscasualflag = false;
    bool isrankflag = false;
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
    void UpdateFlag()
    {
    //Trigger
    if (iscasualflag) {IsCasualQueue.SetActive(true);}
    else {IsCasualQueue.SetActive(false);}
    if (isrankflag) {IsRankQueue.SetActive(true);}
    else {IsRankQueue.SetActive(false);}
    }
}
