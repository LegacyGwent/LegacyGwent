using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43020")]//增援
    public class Reinforcement : CardEffect
    {//从牌组打出1张铜色/银色“士兵”、“机械”、“军官”或“辅助”单位牌。
        public Reinforcement(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.HasAnyCategorie(Categorie.Soldier, Categorie.Machine, Categorie.Support, Categorie.Officer) &&
                   (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                .Mess(Game.RNG)
                .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
            //选一张，如果没选，什么都不做
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0)
            {
                return 0;
            }

            //打出
            var playCard = cards.Single();
            await playCard.MoveToCardStayFirst();
            return 1;
        }
    }
}