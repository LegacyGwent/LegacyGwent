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
                        var initialArmor = GetInitialArmor(moveCard.Status.CardId);
                        if (initialArmor > 0 && moveCard.Status.CardRow.IsOnPlace())
                        {
                            await moveCard.Effect.Armor(initialArmor, Card);
                        }
                }
                    
            }
            return 0;
        }

        private static int GetInitialArmor(string cardId)
        {
            return cardId switch
            {
                "34004" => 2,
                "34024" => 2,
                "44001" => 4,
                "44003" => 3,
                "44006" => 4,
                "44009" => 2,
                "44010" => 2,
                "44013" => 2,
                "44024" => 1,
                "64022" => 2,
                _ => 0
            };
        }
    }
}
