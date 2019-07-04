using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12038")]//哈马维的梦境
    public class HanmarvynSDream : CardEffect
    {//生成对方墓场中1张非领袖金色单位牌的原始同名，并使其获得2点增益。
        public HanmarvynSDream(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var ids = Game.PlayersCemetery[AnotherPlayer].Where(x => x.Is(Group.Gold)).Select(x => x.Status.CardId).ToArray();
            var count = await Game.CreateAndMoveStay(PlayerIndex, ids);
            if(count==0)
            {
                return 0;
            }
            await Game.PlayersStay[PlayerIndex][0].Effect.Boost(2, Card);
            return 1;
        }
    }
}