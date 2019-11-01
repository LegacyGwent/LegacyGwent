using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22002")]//战灵
    public class Draug : CardEffect
    {//将死去的单位复活为战力为1的“战鬼”，直至填满此排。
        public Draug(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = Card.Status.CardRow;
            while (!(Game.RowToList(PlayerIndex, row).Count >= Game.RowMaxCount || Game.PlayersCemetery[PlayerIndex].Count <= 0))
            {
                if (!Game.PlayersCemetery[PlayerIndex].TryMessOne(out var target, Game.RNG))
                {
                    return 0;
                }
                await target.Effect.Transform(CardId.Draugir, Card, x => x.Status.Strength = 1);
                await target.Effect.Resurrect(new CardLocation(row, int.MaxValue), Card);
            }
            return 0;
        }
    }
}