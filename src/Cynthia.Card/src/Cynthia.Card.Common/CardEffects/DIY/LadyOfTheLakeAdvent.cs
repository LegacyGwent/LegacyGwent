using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70011")]//湖中仙女：降临
    public class LadyOfTheLakeAdvent : CardEffect
    {//生成一张湖中仙女（25战力，部署：对自身造成等同于己方牌组中剩余牌数量与手牌数量之和的削弱。）
        public LadyOfTheLakeAdvent(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            await Game.CreateToStayFirst(CardId.LadyOfTheLake, Card.PlayerIndex);
            return 1;
        }
    }
}