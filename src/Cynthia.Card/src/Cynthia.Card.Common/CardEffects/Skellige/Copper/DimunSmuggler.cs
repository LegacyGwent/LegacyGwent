using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;
namespace Cynthia.Card
{
    [CardEffectId("64004")]//迪门家族走私贩
    public class DimunSmuggler : CardEffect
    {//将1个铜色单位从己方墓场返回至牌组。
        public DimunSmuggler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            {
                var list = Game.PlayersCemetery[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit).ToList();
                //让玩家选择
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2, isCanOver: true);
                foreach (var x in result.ToList())
                {
                    var playerIndex = x.PlayerIndex;
                    x.Effect.Repair();
                    var range = Game.RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count() + 1);
                    await x.Effect.Resurrect(new CardLocation(RowPosition.MyDeck, range), x);
                }
                return 0;
            }
        }
    }
}