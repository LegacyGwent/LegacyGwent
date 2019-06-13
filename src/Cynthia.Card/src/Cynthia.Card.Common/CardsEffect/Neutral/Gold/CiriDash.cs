using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12005")]//希里：冲刺
    public class CiriDash : CardEffect
    {//被置入墓场时返回牌组，并获得3点强化。
        public CiriDash(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task OnCardToCemetery(GameCard taget, CardLocation soure)
        {
            if (taget != Card) return;
            var range = new Random().Next(0, Game.PlayersHandCard[PlayerIndex].Count() + 1);
            await Card.Effect.Resurrect(new CardLocation(RowPosition.MyDeck, range), Card);
            await Card.Effect.Strengthen(3, Card);
        }
    }
}