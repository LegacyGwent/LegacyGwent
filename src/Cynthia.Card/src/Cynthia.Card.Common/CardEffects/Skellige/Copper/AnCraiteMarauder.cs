using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64020")]//奎特家族劫掠者
    public class AnCraiteMarauder : CardEffect
    {//造成4点伤害。若被复活，则造成6点伤害。
        public AnCraiteMarauder(GameCard card) : base(card) { }
        private bool _resurrectedflag = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {	
			//选取一个任意单位
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(_resurrectedflag?6:4, Card);
            return 0;
        }

		
    }
}