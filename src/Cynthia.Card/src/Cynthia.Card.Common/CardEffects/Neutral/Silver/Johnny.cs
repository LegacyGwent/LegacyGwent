using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13008")]//乔尼
    public class Johnny : CardEffect
    {//丢弃1张手牌，并在手牌中添加1张对方起始牌组中颜色相同的原始同名牌。
        public Johnny(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersHandCard[PlayerIndex]);
            if(!cards.TrySingle(out var target))
            {
                return 0;
            }
            var targetGroup = target.Status.Group;
            if(!Game.PlayerBaseDeck[AnotherPlayer].Deck.Where(x=>x.Group==targetGroup).Select(x=>x.CardId).TryMessOne(out var targetId,Game.RNG))
            {
                return 0;
            }
            await target.Effect.Discard(Card);
            await Game.CreateCardAtEnd(targetId, PlayerIndex, RowPosition.MyHand);
            return 0;
        }
    }
}