using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public string RowId;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"被拖上{RowId},坐标是{eventData.position}");
        AddCard(eventData.pointerDrag);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"点入{eventData}");
    }
    public void AddCard(GameObject Card,int index = 0)
    {
        Card.transform.SetParent(transform);
        Card.transform.localScale = Vector3.one * 0.85f;
        Card.GetComponent<TestDrag>().ResetParentPosition();
    }
}
