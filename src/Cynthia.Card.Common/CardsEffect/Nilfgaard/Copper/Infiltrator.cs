using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34008")]//渗透者
    public class Infiltrator : CardEffect
    {
        public Infiltrator(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {   //选择一张卡,如果不为0,修改间谍
            var result = (await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.AllRow));
            if(result.Count!=0) await result.Single().Effect.Spying(Card);
            return 0;
        }
    }
}