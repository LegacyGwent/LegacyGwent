using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70166")]//被诅咒的卷轴 CursedScroll
    public class CursedScroll : CardEffect
    {//
        public CursedScroll(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var goldCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.Status.Group == Group.Gold);

            var silverCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.Status.Group == Group.Silver);

            var copperCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.Status.Group == Group.Copper);

            var list = new List<GameCard>();

            if (goldCandidate != default)
            {
                list.Add(goldCandidate);
            }

            if (silverCandidate != default)
            {
                list.Add(silverCandidate);
            }

            if (copperCandidate != default)
            {
                list.Add(copperCandidate);
            }

            //让玩家选择一张牌
            if (list.Count() == 0)
            {
                return 0;
            }

            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1, "选择打出一张牌");

            //如果玩家一张牌都没选择,
            if (result.Count() == 0)
            {
                return 0;
            }
            await result.Single().MoveToCardStayFirst();
            var resultList = list.Where(x => x != result.Single());
            foreach (var cards in resultList)
            {
                await cards.Effect.Discard(Card);
            }
            //打出所选的牌
            return 1;
        }
    }
}
