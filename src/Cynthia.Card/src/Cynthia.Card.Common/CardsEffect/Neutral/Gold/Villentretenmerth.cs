using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12008")]//维伦特瑞坦梅斯
    public class Villentretenmerth : CardEffect
    {//3回合后的回合开始时：摧毁场上除自身外所有最强的单位。 3点护甲。
        public Villentretenmerth(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            await Card.Effect.Armor(3, Card);
            return 0;
        }
        public override async Task OnTurnStart(int playerIndex)
        {
            if (playerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.Status.Countdown > 0)
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {//触发效果
                    var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x => x.Status.CardRow.IsOnPlace() && x != Card).WhereAllHighest().ToList();
                    foreach (var card in cards)
                    {
                        await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                    }
                }
            }
        }
    }
}