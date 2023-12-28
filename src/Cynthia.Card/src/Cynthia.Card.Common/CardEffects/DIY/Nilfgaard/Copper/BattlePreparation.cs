using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70123")]//战前准备 BattlePreparation
    public class BattlePreparation : CardEffect
    {//从手牌打出1张铜色“士兵”牌，随后抽1张牌。从手牌打出一张铜色士兵牌并使其获得2点增益，随后抽一张牌
        public BattlePreparation(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {

            var handlist = Game.PlayersHandCard[Card.PlayerIndex].Where(x => (x.Status.Group == Group.Copper) && x.HasAnyCategorie(Categorie.Soldier));
            if (handlist.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, handlist.ToList(), 1);
            if (!result.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Boost(2, Card);
            await Game.PlayerDrawCard(Card.PlayerIndex, 1);
            await result.Single().MoveToCardStayFirst();

            return 1;
        }
    }
}
