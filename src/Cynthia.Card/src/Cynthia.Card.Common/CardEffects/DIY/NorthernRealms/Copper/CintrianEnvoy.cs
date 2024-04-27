using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70144")]//辛特拉使者
    public class CintrianEnvoy : CardEffect
    {//择一：打出1张自身的同名牌。
        public CintrianEnvoy(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            if (list.Count() == 0)
            {
                await Game.CreateCardAtEnd(Card.Status.CardId, PlayerIndex, RowPosition.MyDeck);
                return 0;
            }
            await list.First().MoveToCardStayFirst();
            return 1;
        }
    }
}

