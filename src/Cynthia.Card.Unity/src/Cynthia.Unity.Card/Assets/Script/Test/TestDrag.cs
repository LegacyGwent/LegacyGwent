using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestDrag : MonoBehaviour, IDragHandler,IEndDragHandler,IBeginDragHandler
{
    private Vector2 _tagetPosition;
    public Vector2 ResetPosition { get; set; }
    private Vector2 _parentPosition;
    private bool _isDrag = false;

    private void Start()
    {
        ResetParentPosition();
    }
    public void ResetParentPosition()
    {
        _parentPosition = transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mouse = _tagetPosition = ResolutionChange(eventData.position);
        _tagetPosition = new Vector2(mouse.x - _parentPosition.x, mouse.y - _parentPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
    }

    private void Update()
    {
        var position = gameObject.GetComponent<RectTransform>().anchoredPosition;
        if (_isDrag)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(position, _tagetPosition, 0.15f);
            return;
        }
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(position, ResetPosition, 0.15f);
    }
    public Vector2 ResolutionChange(Vector2 WorldPosition)
    {
        var scale = 1920f / Screen.width;
        var gameHeight = (Screen.height - 1080f / scale)*2;
        return new Vector2(WorldPosition.x*scale,WorldPosition.y*scale-gameHeight);
    }
}
