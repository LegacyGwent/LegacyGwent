using UnityEngine;
using DG.Tweening;
using Autofac;
using Cynthia.Card.Client;

public class EditorShowDeck : MonoBehaviour
{
    public RectTransform ButtonsContext;
    public RectTransform DeckEditor;
    public MainCodeService _codeService;
    public string Id { get; set; }

    public bool IsShow { get => _isShow;
        set
        {
            if (_isShow == value) return;//如果状态设置一致,不做出改变
            _isShow = value;//否则根据状态改变
            if (_isShow)
            {
                var count = transform.parent.childCount;
                for(var i = 0; i<count; i++)
                {
                    var item = transform.parent.GetChild(i);
                    if(item!=transform&&item.gameObject.GetComponent<EditorShowDeck>()!=null&& item.gameObject.GetComponent<EditorShowDeck>().IsShow)
                    {
                        item.gameObject.GetComponent<EditorShowDeck>().IsShow = false;
                    }
                }
                OpenButtons();
            }
            else CloseButtons();
        }}
    private bool _isShow;

    private void Awake()
    {
        _codeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }

    public void DeckClick()
    {
        IsShow = !IsShow;
    }
    public void RemoveClick()
    {
        _codeService.GetCode<MainCode>().EditorMenu.ShowDeckRemoveClick(Id);
    }
    public void EditorClick()
    {
        _codeService.GetCode<MainCode>().EditorMenu.ShowDeckEditorClick(Id);
    }
    //-----------------------------
    public void OpenButtons()
    {
        DOTween.To(() => ButtonsContext.anchoredPosition, x =>
        {
             ButtonsContext.anchoredPosition = x;
            _codeService.GetCode<MainCode>().EditorMenu.ShowDecksContext.sizeDelta += Vector2.one;
        },new Vector2(0, 0), 0.3f);//打开按钮
        DOTween.To(() => DeckEditor.sizeDelta, x =>
        {
            DeckEditor.sizeDelta = x;
            _codeService.GetCode<MainCode>().EditorMenu.ShowDecksContext.sizeDelta -= Vector2.one;
        }, new Vector2(345, 160),0.3f);
    }
    public void CloseButtons()
    {
        DOTween.To(() => ButtonsContext.anchoredPosition, x => 
        {
            ButtonsContext.anchoredPosition = x;
            _codeService.GetCode<MainCode>().EditorMenu.ShowDecksContext.sizeDelta += Vector2.one;
        },new Vector2(0, 80), 0.3f);//打开按钮
        DOTween.To(() => DeckEditor.sizeDelta, x => 
        {
            DeckEditor.sizeDelta = x;
            _codeService.GetCode<MainCode>().EditorMenu.ShowDecksContext.sizeDelta -= Vector2.one;
        }, new Vector2(345, 80), 0.3f);
    }
}
