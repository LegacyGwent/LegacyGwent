using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13036")]//召唤法阵
    public class SummoningCircle : CardEffect, IHandlesEvent<AfterUnitDown>
    {//生成1张上张被打出的铜色/银色非“密探”单位牌的原始同名牌。
        public SummoningCircle(GameCard card) : base(card) { }

        private static GameCard _create = null;
        public override async Task<int> CardUseEffect()
        {
            // var cards = Game.HistoryList
            //     .Where(x => ((!x.CardId.CardInfo().Categories.Contains(Categorie.Agent)) &&
            //                 (x.CardId.CardInfo().Group == Group.Copper || x.CardId.CardInfo().Group == Group.Silver) &&
            //                 (x.CardId.CardInfo().CardType == CardType.Unit)))
            //                 .ToList();
            // if (cards.Count() <= 0) return 0;
            if (_create == null)
            {
                return 0;
            }
            // await Game.CreateCard(cards.Last().CardId, PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            await Game.CreateCard(_create.Status.CardId, PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.IsMoveInfo.isMove || !(@event.Target.IsAnyGroup(Group.Copper, Group.Silver) && @event.Target.Status.Type == CardType.Unit) || @event.Target.Status.Categories.HasAny(Categorie.Agent))
            {
                return;
            }
            _create = @event.Target;
            await Task.CompletedTask;
        }
    }
}