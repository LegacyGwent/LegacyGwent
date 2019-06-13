using Autofac;
using Cynthia.Card.Client;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorListCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MainCodeService MainCodeService { get; set; }
    public string Id { get; set; }

    private void Start()
    {
        MainCodeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        MainCodeService.ClickEditorListCard(Id);
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        //MainCodeService.SelectSwitchUICard(CardShowInfo.CurrentCore);
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        //MainCodeService.SelectSwitchUICard(CardShowInfo.CurrentCore, false);
    }
}
