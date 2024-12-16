using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;
using Assets.Script.Localization;
using Autofac;

public class popup : MonoBehaviour
{
    [SerializeField] Text ggmessage;
    private string gg_message;
    private LocalizationService _translator;
    void Start()
    {
         _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        GameObject g = GameObject.Find("GameResult");
        var ResultScript = g.GetComponent<GameResultControl>();
        string gg_message1 = _translator.GetText("GG_Message1");
        string gg_message2 = _translator.GetText("GG_Message2");
        string opponent = ResultScript.enemyname;
        gg_message = gg_message1 + opponent + gg_message2;
        ggmessage.text = gg_message;
        Destroy(gameObject, 5f);

    }
}
