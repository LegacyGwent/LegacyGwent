using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script disables the flag when the play button is clicked and reactivates it after 3 seconds to avoid a flag trigered by you queing yourself
public class SelfFlag : MonoBehaviour
{
    public GameObject modeflag;
    public GameObject nonmodeflag;

    bool isclicked = false;

    public async void PlayButtonClicked()
    {
        if (isclicked == false)
        {
            modeflag.SetActive(false);
            nonmodeflag.SetActive(false);
            isclicked = true;
        }
        else
        {
            this.Invoke("ToggleFlags", 3f);
        }
    }
    private void ToggleFlags()
    {
        if (isclicked == false)
            return;
        modeflag.SetActive(true);
        nonmodeflag.SetActive(true);
        isclicked = false;
    }
}
