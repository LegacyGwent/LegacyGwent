using System.Collections;
using System.Collections.Generic;

namespace Cynthia.Card
{
    public interface ICardsPosition
    {
        float Size { get; set; }
        float Width { get; set; }
        bool IsCanDrag { get; set; }//其中卡牌是否可拖动
        bool IsCanSelect { get; set; }//其中卡牌是否可被选中
        int MaxCards { get; set; }
        int GetCardCount();//卡牌的数量
        void ResetCards();//固定位置
        void CardsCanDrag(bool isCanDrag);//设定是否可拖动
        void CardsCanSelect(bool isCanSelect);//设定是否可选中
        void AddCard(CardMoveInfo card, int cardIndex);//在指定位置添加卡牌
        void RemoveCard(int cardIndex);//删除卡牌
        void CreateCard(CardMoveInfo card, int cardIndex);//指定位置创建卡牌
        void SetCards(IEnumerable<CardMoveInfo> Cards);//设定初始卡牌
        void SetCards(IEnumerable<GameCard> Cards);//设定初始卡牌
    }
    public interface ITemCard
    {
        bool IsTem();
        void AddTemCard(GameCard cardInfo, int index);
    }
}