using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70153")]//莫拉汉姆家猎手 VanMoorleheHunter
    public class VanMoorleheHunter : CardEffect
    {//
        public VanMoorleheHunter(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var hand = Game.PlayersHandCard[Card.PlayerIndex].Where(x=>(x.IsAnyGroup(Group.Gold)));
            int rcount = 1;
            for(var i = 0; i < rcount;i++)
            {
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                if (!selectList.TrySingle(out var target))
                {
                    return 0;
                }
                await target.Effect.Damage(3,Card);
                if(hand.Count() == 0)
                {
                    rcount = 2;
                }
            }
            return 0;
        }
    }
}
