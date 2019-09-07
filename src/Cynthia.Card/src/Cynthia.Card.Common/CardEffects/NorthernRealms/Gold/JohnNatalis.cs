using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42005")]//约翰·纳塔利斯
    public class JohnNatalis : CardEffect
    {//从牌组打出1张铜色/银色“谋略”牌。
        public JohnNatalis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //乱序列出谋略，如果没有，什么都不做
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Tactic) &&
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