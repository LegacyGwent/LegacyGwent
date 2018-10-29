using System.Threading.Tasks;

namespace Cynthia.Card
{
    public static class GameFunctionalExtensions
    {
        public static GwentCard CardInfo(this GameCard card) => GwentMap.CardMap[card.Status.CardId];
        public static GwentCard CardInfo(this CardStatus card) => GwentMap.CardMap[card.CardId];
        public static GwentCard CardInfo(this string cardId) => GwentMap.CardMap[cardId];
        public static Task MoveToCardStayFirst(this GameCard card, bool isShowToEnemy = true)//移动到卡牌移动区末尾
        {
            var game = card.Effect.Game;
            return game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, card, isShowToEnemy);
        }
    }
}