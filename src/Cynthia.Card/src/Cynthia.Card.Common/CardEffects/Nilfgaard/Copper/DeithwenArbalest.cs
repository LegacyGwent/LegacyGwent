using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34015")]//戴斯文强弩手
    public class DeithwenArbalest : CardEffect
    {//对1个敌军单位造成4点伤害。若它为间谍，则伤害提升至8点。
        public DeithwenArbalest(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.EnemyRow);
            if (cards.Count() == 0) return 0;
            var card = cards.Single();
            if (card.Status.IsSpying)
            {
                await card.Effect.Damage(8, Card);
            }
            else
            {
                await card.Effect.Damage(4, Card);
            }
            return 0;
        }
    }
}