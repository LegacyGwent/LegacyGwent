using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12008")]//维伦特瑞坦梅斯
    public class Villentretenmerth : CardEffect, IHandlesEvent<AfterTurnStart>
    {//3回合后的回合开始时：摧毁场上除自身外所有最强的单位。 3点护甲。
        public Villentretenmerth(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            await Card.Effect.Armor(3, Card);
            await Card.Effect.SetCountdown(value: 3);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.Status.Countdown > 0)
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