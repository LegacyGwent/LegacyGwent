using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62009")]//乌弗海登
    public class Ulfhedinn : CardEffect
    {//对所有敌军单位造成1点伤害，已受伤单位则承受2点伤害。
        public Ulfhedinn(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //参考山崩
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex).Mess(Game.RNG).ToList();

            foreach (var card in cards)
            {
                if (card.Status.CardRow.IsOnPlace() && card.Status.HealthStatus >= 0)
                {
                    await card.Effect.Damage(1, Card);
                }
                else if (card.Status.CardRow.IsOnPlace() && card.Status.HealthStatus < 0)
                {
                    await card.Effect.Damage(3, Card);
                }

            }
            return 0;
        }
    }
}