using Cynthia.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanDrop : MonoBehaviour
{
    public RowPosition Id;
    private bool _isCanDrop;
    public CardsPosition CardsPosition;
    public bool IsRowDrop;
    public bool IsCanDrop
    {
        get => _isCanDrop;
        set
        {
            _isCanDrop = value; //&& (!IsRowDrop||CardsPosition.MaxCards > CardsPosition.GetCardCount());
            DropShow.SetActive(_isCanDrop);
        }
    }
    public void IfMaxDotDrop()
    {
        if (IsCanDrop)
        {
            if (CardsPosition == null || Id == RowPosition.SpecialPlace) return;
            if (CardsPosition.MaxCards <= CardsPosition.GetTrueCardCount())
            {
                IsCanDrop = false;
            }
        }
    }

    public GameObject DropShow;

    private void Start()
    {
        IsCanDrop = false;
    }
}
