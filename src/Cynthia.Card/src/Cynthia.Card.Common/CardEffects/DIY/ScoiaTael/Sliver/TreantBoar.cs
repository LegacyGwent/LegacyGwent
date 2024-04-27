using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70139")]//树人野猪 TreantBoar
    public class TreantBoar : CardEffect
    {//
        public TreantBoar(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int rcount = 1;

            for(var i = 0; i < rcount;i++)
            {
                var result = await Game.GetSelectPlaceCards(Card);
                if (!result.Any()) return 0;
                var target1 = result[0];
                await target1.Effect.Damage(3, Card);
                if (!target1.IsAliveOnPlance())
                {
                    rcount = 2;
                }
                var targetRow = TurnType.My.GetRow();
                var row = target1.Status.CardRow;
                targetRow.Remove(row.IsMyRow() ? row : row.Mirror());
                
                var Ltaget = target1.GetRangeCard(1, GetRangeType.HollowLeft);
                if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
                {
                    var canMoveRow = targetRow.Where(x => Game.RowToList(Ltaget.Single().PlayerIndex, x).Count < Game.RowMaxCount);
                    if (!canMoveRow.TryMessOne(out var target, Game.RNG))
                    {
                        return 0;
                    }
                    await Ltaget.Single().Effect.Move(new CardLocation(target, Game.RowToList(Ltaget.Single().PlayerIndex, target).Count), Card);
                }
                var Rtaget = target1.GetRangeCard(1, GetRangeType.HollowRight);
                if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
                {
                    var canMoveRow = targetRow.Where(x => Game.RowToList(Rtaget.Single().PlayerIndex, x).Count < Game.RowMaxCount);
                    if (!canMoveRow.TryMessOne(out var target, Game.RNG))
                    {
                        return 0;
                    }
                    await Rtaget.Single().Effect.Move(new CardLocation(target, Game.RowToList(Rtaget.Single().PlayerIndex, target).Count), Card);
                }
            }

            return 0;
            
        }
    }
}
