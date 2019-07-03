using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13043")]//龙之梦
    public class DragonSDream : CardEffect
    {//在对方单排降下灾厄，当任一玩家打出一张非同名“特殊”牌时，对该排上所有单位造成4点伤害。
        public DragonSDream(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card,
                TurnType.Enemy.GetRow());
            // await Game.ApplyWeather(Card.PlayerIndex,result,RowStatus.DragonDream);
            await Game.GameRowEffect[Game.AnotherPlayer(Card.PlayerIndex)][result.Mirror().MyRowToIndex()]
                .SetStatus<DragonDreamStatus>();
            return 0;
        }
    }
}