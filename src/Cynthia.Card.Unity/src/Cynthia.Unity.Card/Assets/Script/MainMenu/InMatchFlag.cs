using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InMatchFlag : MonoBehaviour
{
    public GameObject modeflag;
    public GameObject nonmodeflag;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void CasualButtonClicked()
    {
        modeflag.SetActive(true);
        nonmodeflag.SetActive(false);
    }
        public void RankButtonClicked()
    {
        modeflag.SetActive(false);
        nonmodeflag.SetActive(true);
    }
}
