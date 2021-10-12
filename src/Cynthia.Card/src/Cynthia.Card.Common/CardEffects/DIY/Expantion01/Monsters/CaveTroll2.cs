using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70058")]//洞穴巨魔 CaveTroll
    public class CaveTroll2 : CardEffect
    {//回合开始时，使1个战力最高的敌军单位获得3点增益，然后自身获得3点增益。
        public CaveTroll2(GameCard card) : base(card) { }
        
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            await Boost(4,Card);
            var cards = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.EnemyRow);
            if (cards.Count() == 0) return 0;
            await cards.Single().Effect.Boost(4, cards.Single());
            return 0;
        }
    }
}
