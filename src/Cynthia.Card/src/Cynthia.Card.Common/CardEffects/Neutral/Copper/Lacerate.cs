using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("14012")]//撕裂
    public class Lacerate : CardEffect
    {//对单排所有单位造成3点伤害。
        public Lacerate(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.All.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, result).IgnoreConcealAndDead();
            foreach (var card in row)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    await card.Effect.Damage(3, Card);
                }
            }
            return 0;
        }
    }
}