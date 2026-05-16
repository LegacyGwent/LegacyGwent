using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70083")]//红骑士
    public class RedRider : CardEffect, IHandlesEvent<BeforeCardToCemetery>, IHandlesEvent<AfterCardToCemetery>
    {//每当有位于“刺骨冰霜”之下的敌军单位被摧毁时，从牌组召唤1张它的同名牌。
        public RedRider(GameCard card) : base(card) { }
        private bool IsToSummon = false;
        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            if (@event.Target.Status.Type == CardType.Unit && @event.Target.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInDeck())
            {
                if (!@event.DeathLocation.RowPosition.IsOnPlace())
                {
                    return;
                }

                bool isInFrost = Game.GameRowEffect[@event.Target.PlayerIndex][@event.DeathLocation.RowPosition.MyRowToIndex()].RowStatus == RowStatus.BitingFrost;
                if (isInFrost)
                {
                    var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                    if (list.Count() == 0)
                    {
                        return;
                    }
                    //只召唤最后一个
                    if (Card == list.First())
                    {
                        IsToSummon = true;
                    }
                }
            }
        }
        public async Task HandleEvent(AfterCardToCemetery @event)
        {//move the actual summon to a separate event to avoid summoning all copies of this card when only one enemy dies to frost
            if (!IsToSummon)
            {
                return;
            }
            if (@event.Target.Status.Type == CardType.Unit && @event.Target.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInDeck())
            {
                IsToSummon = false;
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
            }
            return;
        }

    }
}
