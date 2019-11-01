using System.Collections;
using System.Collections.Generic;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.EventSystems;

public class MatchListLeader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MainCodeService _mainCodeService;
    private void Start()
    {
        _mainCodeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        _mainCodeService.SetMatchArtCard(new CardStatus(gameObject.GetComponent<LeaderShow>().CurrentId));
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        _mainCodeService.SetMatchArtCard(new CardStatus(gameObject.GetComponent<LeaderShow>().CurrentId), false);
    }
}
