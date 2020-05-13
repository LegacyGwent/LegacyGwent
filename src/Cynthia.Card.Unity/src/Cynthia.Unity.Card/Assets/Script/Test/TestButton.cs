using System.Collections;
using System.Collections.Generic;
using Autofac;
using UnityEngine;
using Cynthia.Card.Client;
using Cynthia.Card;
using UnityEngine.UI;
using Alsein.Extensions;
using System.Linq;
using UnityEngine.SceneManagement;

public class TestButton : MonoBehaviour
{
    public ArtCard Card;
    public void Start()
    {
        // SetRNGCard();
        // Text.text = DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay ? "点击停止自动出牌" : "点击启动自动出牌";
    }
    public void Click()
    {
        SceneManager.LoadScene("LoginSecen");
        // SetRNGCard();
        //TestItem.GetComponent<GameEvent>()
        //.CreateCard(new CardStatus(), new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = 0 });
        // DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay = !DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay;
        // Text.text = DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay ? "点击停止自动出牌" : "点击启动自动出牌";
    }
    public void SetRNGCard()
    {
        var rCardStatus = new CardStatus(GwentMap.CardMap.Mess().First().Key);
        Card.CurrentCore = rCardStatus;
    }
}
