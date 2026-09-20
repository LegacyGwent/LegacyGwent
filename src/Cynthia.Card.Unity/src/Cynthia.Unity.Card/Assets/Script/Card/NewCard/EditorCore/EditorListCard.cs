using Autofac;
using Cynthia.Card.Client;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorListCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private MainCodeService _mainCodeService;
    public string Id { get; set; }
    public bool IsPremium { get; set; }

    private void Start()
    {
        _mainCodeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) _mainCodeService.ClickEditorListCard(Id, IsPremium);
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        _mainCodeService.SelectSwitchUICard(gameObject.GetComponent<ListCardShowInfo>().CardStatus);
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        _mainCodeService.SelectSwitchUICard(gameObject.GetComponent<ListCardShowInfo>().CardStatus, false);
    }
}
