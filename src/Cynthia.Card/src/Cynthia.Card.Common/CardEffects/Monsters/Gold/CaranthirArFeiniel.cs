using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22004")]//卡兰希尔
    public class CaranthirArFeiniel : CardEffect
    {//将1个敌军单位移至对方同排，并在此排降下“刺骨冰霜”。
        public CaranthirArFeiniel(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow)).TrySingle(out var target))
            {
                return 0;
            }
            var row = await Game.GetSelectRow(PlayerIndex, Card, TurnType.Enemy.GetRow().Where(row => row != target.Status.CardRow).ToList());
            await target.Effect.Move(new CardLocation(row, int.MaxValue), Card);
            await Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()].SetStatus<BitingFrostStatus>();

            return 0;
        }
    }
}