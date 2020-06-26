using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70004")]//格莱尼斯·爱普·洛纳克

    public class GlynnisAepLoernach : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，如果位于牌组顶或底，则召唤自身至随机排
        public GlynnisAepLoernach(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
            {
                var index = Card.GetLocation().CardIndex;
                if (index == Game.PlayersDeck.Count() - 1 || index == 0)
                {
                    var summonPosition = Game.GetRandomCanPlayLocation(Card.PlayerIndex, false);//, RowPosition.MyRow1);
                    await Summon(summonPosition, Card);
                }
            }
        }
        // public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        // {
        //     await Armor(1, Card);
        //     var list = await Game.GetSelectPlaceCards(Card,
        //         filter: x => x.IsAnyGroup(Group.Copper, Group.Silver),
        //         selectMode: SelectModeType.EnemyRow);

        //     //如果没有，什么都不发生
        //     if (!list.TrySingle(out var target))
        //     {
        //         return 0;
        //     }

        //     var threshold = GwentMap.CardMap[Card.Status.CardId].Strength;

        //     if (threshold > target.CardPoint())
        //     {
        //         await target.Effect.Charm(Card);
        //     }
        //     else
        //     {
        //         await Duel(target, Card);
        //     }

        //     return 0;
        // }
    }
}
