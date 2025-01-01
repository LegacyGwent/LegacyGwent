using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Cynthia.Card;
using Assets.Script.Localization;
using Autofac;

public class popup : MonoBehaviour // get the name of the sender of the GG and return the GG message
{
    [SerializeField] Text ggmessage;
    private string gg_message;
    private LocalizationService _translator;
    public void Start()
    {
         _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        string gg_message1 = _translator.GetText("GG_Message1");
        string gg_message2 = _translator.GetText("GG_Message2");
        string opponent = GGSender.ggsender;
        gg_message = gg_message1 + opponent + gg_message2;
        ggmessage.text = gg_message;
        Destroy(gameObject, 5f);

    }
}
