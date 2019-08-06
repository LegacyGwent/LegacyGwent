using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("64023")]//奎特家族战吼者
    public class AnCraiteWarcrier : CardEffect
    {//使1个友军单位获得自身一半战力的增益。
        public AnCraiteWarcrier(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)

        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            //增益值向下取等
            int boostnum = (target.Status.Strength + target.Status.HealthStatus) / 2;
            await target.Effect.Boost(boostnum, Card);
            return 0;
        }
    }
}