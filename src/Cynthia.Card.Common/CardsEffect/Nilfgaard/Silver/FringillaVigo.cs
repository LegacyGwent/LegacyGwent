using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33018")]//芙琳吉拉·薇歌
    public class FringillaVigo : CardEffect
    {//间谍。将左侧单位的战力复制给右侧单位。
        public FringillaVigo(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var rowIndex = Card.GetLocation(Card.PlayerIndex).CardIndex;
            var list = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).ToList();
            var target = Card.GetRangeCard(1, GetRangeType.HollowAll);
            if (target.Count == 2)
            {
                var left = target.First();
                var right = target.Last();
                int point = left.ToHealth().health - right.ToHealth().health;
                if (point > 0)
                    await right.Effect.Boost(point, Card);
                if (point < 0)
                    await right.Effect.Damage(Math.Abs(point), Card, BulletType.RedLight, true);
            }
            return 0;
        }
    }
}