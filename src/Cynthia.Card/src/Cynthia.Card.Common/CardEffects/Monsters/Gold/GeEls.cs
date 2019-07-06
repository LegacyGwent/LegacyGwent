using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22013")]//盖尔
    public class GeEls : CardEffect
    {//检视牌组中1张金色牌和银色牌，打出1张，将另1张置于牌组顶端。
        public GeEls(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //先选出一张金，一张银
            var goldCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.Status.Group == Group.Gold);

            var silverCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.Status.Group == Group.Silver);

            var list = new List<GameCard>();

            if (goldCandidate != default)
            {
                list.Add(goldCandidate);
            }

            if (silverCandidate != default)
            {
                list.Add(silverCandidate);
            }

            //让玩家选择一张牌
            if (list.Count() == 0)
            {
                return 0;
            }

            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1, "选择打出一张牌");

            //如果玩家一张牌都没选择, 两张都置于牌库顶
            if (result.Count() == 0)
            {
                foreach (var cardToMove in list)
                {
                    await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), cardToMove);
                }
                return 0;
            }

            //如果玩家选了牌
            //如果有另一张牌，先移动另一张牌到牌库顶
            if (list.Count() == 2)
            {
                var cardToMove = list.Where(x => x != result.Single()).Single();
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), cardToMove);
            }

            //打出所选的牌
            await result.Single().MoveToCardStayFirst();

            return 1;
        }
    }
}