using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowEvent : MonoBehaviour
{
    public CardsPosition CardsPosition;
    public bool IsCanDrag { get => CardsPosition.IsCanDrag; set => CardsPosition.IsCanDrag = value; }
    public void ResetCards() => CardsPosition.ResetCards();
    public void AddCard(CardMoveInfo card, int cardIndex) => CardsPosition.AddCard(card, cardIndex);
    public void CardsCanDrag(bool isCanDrag) => CardsPosition.CardsCanDrag(isCanDrag);
}
