using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23012")]//蟾蜍王子
    public class ToadPrince : CardEffect
    {//抽1张单位牌，随后吞噬1张手牌中的单位牌，获得等同于其战力的增益。
        public ToadPrince(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.PlayerDrawCard(PlayerIndex, 1, x => x.Status.Type == CardType.Unit);
            var cardlist = Game.PlayersHandCard[PlayerIndex].Where(x => x.Status.Type == CardType.Unit).ToList();
            var targetcards = await Game.GetSelectMenuCards(PlayerIndex, cardlist, isCanOver: false);
            if (!targetcards.TrySingle(out var target))
            {
                return 0;
            }
            await Consume(target);
            return 0;
        }
    }
}