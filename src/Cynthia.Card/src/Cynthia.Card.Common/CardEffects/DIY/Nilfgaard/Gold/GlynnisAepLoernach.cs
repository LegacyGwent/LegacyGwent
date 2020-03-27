using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70004")]//格莱尼斯·爱普·洛纳克

    public class GlynnisAepLoernach : CardEffect
    {//2护甲。部署：与一个敌军铜/银色单位对决。若自身初始战力高于对方，则改为将其魅惑。
        public GlynnisAepLoernach(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Armor(2, Card);
            var list = await Game.GetSelectPlaceCards(Card,
                filter: x => x.IsAnyGroup(Group.Copper, Group.Silver),
                selectMode: SelectModeType.EnemyRow);

            //如果没有，什么都不发生
            if (!list.TrySingle(out var target))
            {
                return 0;
            }

            var threshold = GwentMap.CardMap[Card.Status.CardId].Strength;

            if (threshold > target.CardPoint())
            {
                await target.Effect.Charm(Card);
            }
            else
            {
                await Duel(target, Card);
            }
            
            return 0;
        }
    }
}
