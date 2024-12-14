using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;

public class popup : MonoBehaviour
{
    [SerializeField] Text ggmessage;
    private string gg_message;
    void Start()
    {
        GameObject g = GameObject.Find("GameResult");
        var ResultScript = g.GetComponent<GameResultControl>();
        string opponent = ResultScript.enemyname;
        gg_message = $"Good Game! Your opponent {opponent} sent GG!";
        ggmessage.text = gg_message;
        Destroy(gameObject, 5f);

    }
}
