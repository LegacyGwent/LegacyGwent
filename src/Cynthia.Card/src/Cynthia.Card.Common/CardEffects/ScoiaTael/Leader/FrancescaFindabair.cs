using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("51001")]//法兰茜丝卡
    public class FrancescaFindabair : CardEffect
    {//选择1张牌进行交换，交换所得的卡牌获得3点增益。
        public FrancescaFindabair(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //乱序列出牌库中牌
            var deckselectlist = Game.PlayersDeck[Card.PlayerIndex].Mess(RNG).ToList();
            if (deckselectlist.Count() == 0)
            {
                return 0;
            }
            //手牌列表
            var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, selectList)).TrySingle(out var swapHandCard))
            {
                return 0;
            }
            //选一张，必须选
            var swapdeckcard = await Game.GetSelectMenuCards(Card.PlayerIndex, deckselectlist, 1, isCanOver: false);
            //交换
            await swapHandCard.Effect.Swap(swapdeckcard.Single());

            await swapdeckcard.Single().Effect.Boost(3, Card);
            return 0;
        }
    }
}