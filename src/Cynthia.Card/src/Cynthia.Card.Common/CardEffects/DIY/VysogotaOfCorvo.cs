using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70005")]//科沃的维索戈塔
    public class VysogotaOfCorvo : CardEffect, IHandlesEvent<AfterCardDeath>
    {//部署：治愈1个友军单位，然后使其获得“免疫”。遗愿：所有友军单位失去“免疫”。
        public VysogotaOfCorvo(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target)) { return 0; }
            await target.Effect.Heal(Card);
            await target.Status.IsImmue = true;
        }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target == Card)
            {
                var cards = Game.GetPlaceCards(Card.PlayerIndex)
                        .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex).ToList();
                foreach (var card in cards)
                {
                    await card.Status.IsImmue = true;
                }
            }
        }
    }
}