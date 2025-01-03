using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SelfFlag : MonoBehaviour
{
    public GameObject modeflag;
    public GameObject nonmodeflag;

    bool isclicked = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void PlayButtonClicked()
    {
        if (isclicked == false)
        {modeflag.SetActive(false);
        nonmodeflag.SetActive(false);
        isclicked = true;
        }
        else
        {modeflag.SetActive(true);
        nonmodeflag.SetActive(true);
        isclicked = false;
        }
    }
}
