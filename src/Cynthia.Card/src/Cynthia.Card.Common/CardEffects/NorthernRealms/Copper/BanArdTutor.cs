using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44002")]//班·阿德导师
    public class BanArdTutor : CardEffect
    {//用1张手牌交换牌组中的一张铜色“特殊”牌。
        public BanArdTutor(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //乱序列出牌库中铜色特殊牌
            var deckselectlist = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Special) && x.Status.Group == Group.Copper).Mess(RNG).ToList();
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
            return 0;
        }
    }
}