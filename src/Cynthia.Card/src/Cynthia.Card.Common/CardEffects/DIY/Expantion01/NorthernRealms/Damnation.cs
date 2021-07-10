using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70078")]//烈火责罚 Damnation
    public class Damnation : CardEffect
    {//从牌组召唤2张最强铜色单位牌到同排，改变其锁定状态。
        public Damnation(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.My.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, result);
            for (var i = 0; i < 2; i++)
            {
                if (row.Count < Game.RowMaxCount)
                {
                    var list = Game.PlayersDeck[PlayerIndex]
                        .Where(x => ((x.Status.Group == Group.Copper) &&//铜
                                x.CardInfo().CardType == CardType.Unit)).WhereAllHighest().ToList();//单位牌
                        if (list.Count() == 0) return 0;
                        var moveCard = list.Mess(RNG).First();
                        await moveCard.Effect.Lock(Card);
                        await moveCard.Effect.Summon(new CardLocation(result, row.Count), Card);
                }
                    
            }
            return 0;
        }
    }
}