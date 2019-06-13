using System.Collections;
using System.Collections.Generic;
using Autofac;
using UnityEngine;
using Cynthia.Card.Client;
using Cynthia.Card;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    public GameObject TestItem;
    public Text Text;
    public void Start()
    {
        Text.text = DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay ? "点击停止自动出牌" : "点击启动自动出牌";
    }
    public void Click()
    {
        //TestItem.GetComponent<GameEvent>()
            //.CreateCard(new CardStatus(), new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = 0 });
        DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay = !DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay;
        Text.text = DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay ? "点击停止自动出牌" : "点击启动自动出牌";
    }
    private void Update()
    {
    }
}
