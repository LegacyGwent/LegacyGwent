using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70165")]//薇薇恩·塔布里司 ViviennedeTabris
    public class ViviennedeTabris : CardEffect
    {//
        public ViviennedeTabris(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var list = Game.PlayerBaseDeck[PlayerIndex].Deck.Where(x => x.Is(Group.Gold, CardType.Unit) && x.CardId != Card.CardInfo().CardId);
            var targetList = list.Select(x => new CardStatus(x.CardId)).ToList();
            var result = (await Game.GetSelectMenuCards(PlayerIndex, targetList, isCanOver: false, title: "选择一张牌"));
            if (!(result).TrySingle(out var targetIndex))
            {
                return 0;
            }
            var id = targetList[targetIndex].CardId;
            await target.Effect.Transform(id, Card);
            await target.Effect.Boost(2, Card);
            return 0;
        }
    }
}
