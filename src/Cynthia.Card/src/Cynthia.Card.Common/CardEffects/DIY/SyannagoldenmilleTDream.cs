using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("72043")]//席安娜，“黄粱一梦”
    public class SyannagoldenmilleTDream : CardEffect
    {//摧毁场上战力最高的牌。若其已被增益，则使其被放逐。
        public SyannagoldenmilleTDream(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x => x.Status.CardRow.IsOnPlace()).WhereAllHighest().ToList();
            foreach (var card in cards)
            {
                if (card.Status.HealthStatus > 0) 
                {await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                card.Status.IsDoomed = true;
                }
            else
            await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;

        }
    }
}
