using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingScreenStarter : MonoBehaviour
{
    public GameObject StartingScreen;

    private void Start()
    {
        StartingScreen.SetActive(true);
    }
}
