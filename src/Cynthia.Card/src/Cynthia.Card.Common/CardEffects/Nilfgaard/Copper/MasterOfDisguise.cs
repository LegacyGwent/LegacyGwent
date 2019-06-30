using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34006")]//伪装大师
    public class MasterOfDisguise : CardEffect
    {//隐匿2张牌。
        public MasterOfDisguise(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = Game.GetAllCard(PlayerIndex).Where(x => x.Status.IsReveal).ToList();
            var targetCards = await Game.GetSelectMenuCards(PlayerIndex, cards, 2);
            foreach (var card in targetCards)
            {
                await card.Effect.Conceal(Card);
            }
            return 0;
        }
    }
}