using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("14016")]//斯丹莫福德的山崩术
    public class StammelfordSTremor : CardEffect
    {//对所有敌军单位造成1点伤害。
        public StammelfordSTremor(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex).Mess(RNG).ToList();
            foreach (var card in cards)
            {
                if (card.Status.CardRow.IsOnPlace())
                    await card.Effect.Damage(1, Card);
            }
            return 0;
        }
    }
}