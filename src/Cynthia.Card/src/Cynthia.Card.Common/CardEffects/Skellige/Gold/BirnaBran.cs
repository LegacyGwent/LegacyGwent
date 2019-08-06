using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62006")]//碧尔娜·布兰
    public class BirnaBran : CardEffect
    {//在对方单排降下“史凯利格风暴”。
        public BirnaBran(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.Enemy.GetRow());
            // await Game.ApplyWeather(Card.PlayerIndex,result,RowStatus.SkelligeStorm);
            await Game.GameRowEffect[Game.AnotherPlayer(Card.PlayerIndex)][result.Mirror().MyRowToIndex()]
                .SetStatus<SkelligeStormStatus>();
            return 0;
        }
    }
}