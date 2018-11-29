using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34020")]//娜乌西卡旅
    public class NauzicaaBrigade : CardEffect
    {
        public NauzicaaBrigade(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //对1个间谍单位造成7点伤害。若摧毁目标，则获得4点强化。
            var result = (await Game.GetSelectPlaceCards(1, Card,x=>x.Status.IsSpying==true,selectMode:SelectModeType.AllRow)).ToList();
            if(result.Count!=0)
            { 
                await result.Single().Effect.Damage(7,Card);
                if(!result.Single().Status.CardRow.IsOnPlace())
                {
                    await Card.Effect.Strengthen(4,Card);
                }
            }
            return 0;
        }
    }
}