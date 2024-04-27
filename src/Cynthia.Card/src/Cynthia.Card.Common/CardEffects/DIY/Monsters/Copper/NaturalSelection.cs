using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70129")]//物竞天择 NaturalSelection
    public class NaturalSelection : CardEffect
    {//生成2个“蟹蜘蛛幼虫”，墓地中每有1张同名牌额外生成1个。
        public NaturalSelection(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.My.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, result);
            var cardsCount = (Game.PlayersCemetery[Card.PlayerIndex].ToList().Where(x => x.Status.CardId == Card.Status.CardId).Count());
            for (var i = 0; i < 2 + cardsCount; i++)
            {
                if (row.Count < Game.RowMaxCount)
                {
                    await Game.CreateCard("25002", Card.PlayerIndex, new CardLocation(result, row.Count));
                }
            }
            return 0;
        }
    }
}



