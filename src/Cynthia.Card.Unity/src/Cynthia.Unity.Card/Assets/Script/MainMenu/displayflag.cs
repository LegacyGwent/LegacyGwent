using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayflag : MonoBehaviour
{
    public GameObject IsCasualQueue;
    bool iscasualflag = false;
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
    void UpdateFlag()
    {
    //Trigger
    if (iscasualflag) {IsCasualQueue.SetActive(true);}
    else {IsCasualQueue.SetActive(false);}
    }
}
