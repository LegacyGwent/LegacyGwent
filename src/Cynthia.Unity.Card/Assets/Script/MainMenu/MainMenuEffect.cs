using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Message;
    public GameObject Title;

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Animator>().Play("OnMenu");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Animator>().Play("LeaveMenu");
    }
    public void TextReset()
    {
        Message.SetActive(false);
        Title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 40);
    }
}