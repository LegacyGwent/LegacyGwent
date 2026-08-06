using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13004")]//爱丽丝的同伴
    public class IrisCompanions : CardEffect
    {//将1张牌从牌组移至手牌，然后随机丢弃1张牌。
        public IrisCompanions(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //己方卡组乱序呈现
            var list = Game.PlayersDeck[PlayerIndex].Mess(RNG).ToList();
            var result = await Game.GetSelectMenuCards(PlayerIndex, list, isCanOver: true);
            if (result.Count == 0) return 0;//如果没有任何符合标准的牌,返回
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(PlayerIndex);//抽卡
                                                   //---------------------------------------------------------------------------
            var hasIrisShade = Game.GetPlaceCards(PlayerIndex)
                .Any(card => card.Status.CardId == "70154" && !card.Status.IsLock);
            if (hasIrisShade)
            {
                var selected = await Game.GetSelectMenuCards(
                    PlayerIndex,
                    Game.PlayersHandCard[PlayerIndex],
                    isCanOver: false);
                if (selected.TrySingle(out var discardCard))
                {
                    await discardCard.Effect.Discard(Card);
                }
            }
            else
            {
                var discardCard = Game.PlayersHandCard[PlayerIndex].Mess(RNG).First();
                await discardCard.Effect.Discard(Card);
            }
            return 0;
        }
    }
}
