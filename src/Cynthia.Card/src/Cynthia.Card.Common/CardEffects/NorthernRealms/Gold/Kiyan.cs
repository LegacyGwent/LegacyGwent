using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42010")]//凯亚恩
    public class Kiyan : CardEffect
    {//择一：创造1张铜色/银色“炼金”牌；或从牌组打出1张铜色/银色“道具”牌。
        public Kiyan(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var switchCard = await Card.GetMenuSwitch(("禁术", "创造1张铜色/银色“炼金”牌。"), ("忌器", "从牌组打出1张铜色/银色“道具”牌。"));
            if (switchCard == 0)
            {
                var ids = GwentMap.GetCreateCardsId(x => x.Is(filter: x => x.HasAllCategorie(Categorie.Alchemy) && x.IsAnyGroup(Group.Copper, Group.Silver)), Game.RNG);
                return await Game.CreateAndMoveStay(PlayerIndex, ids.ToArray());
            }
            if (switchCard == 1)
            {
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Item) &&
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
            return 0;
        }
    }
}