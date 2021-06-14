using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130190")]//爱丽丝·伊佛瑞克：晋升
    public class IrisVonEverecPro : CardEffect, IHandlesEvent<AfterCardDeath>
    {//间谍。 遗愿：使对面半场6个随机单位获得5点增益。
        public IrisVonEverecPro(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == AnotherPlayer).Mess(RNG).Take(6).ToList();
            foreach (var card in cards)
            {
                if (card.Status.CardRow.IsOnPlace())
                    await card.Effect.Boost(5, Card);
            }
            return;
        }
    }
}