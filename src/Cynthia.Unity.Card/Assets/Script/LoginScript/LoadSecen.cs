using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cynthia.Card;
using Autofac;

public class LoadSecen : MonoBehaviour
{
    public void LoadSecenOfName(string secenName)
    {
        SceneManager.LoadScene(secenName);
    }
}
