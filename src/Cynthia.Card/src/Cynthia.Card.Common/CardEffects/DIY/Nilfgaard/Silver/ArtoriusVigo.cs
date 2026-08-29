using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70103")]//亚托列司·薇歌 ArtoriusVigo
    public class ArtoriusVigo : CardEffect
    {//选择1张手牌，将其转化为1张己方起始牌组中铜色单位牌的原始同名牌，随后将其揭示。
        public ArtoriusVigo(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectMenuCards(
                PlayerIndex,
                Game.PlayersHandCard[PlayerIndex].ToList());
            if (!cards.TrySingle(out var target))
            {
                return 0;
            }

            var list = Game.PlayerBaseDeck[PlayerIndex].Deck.Where(x => x.Is(Group.Copper, CardType.Unit)); 
            var selectList = list
                .Select(x => x.CardId)
                .Distinct()
                .Select(id => new CardStatus(id))
                .ToList();

            var result = await Game.GetSelectMenuCards(PlayerIndex, selectList, isCanOver: false, title: "选择一张牌");
            if (!(result).TrySingle(out var targetIndex))
            {
                return 0;
            }
            var id = selectList[targetIndex].CardId;
            await target.Effect.Transform(id, Card, null, true);
            await target.Effect.Reveal(Card);
            return 0;
        }
    }
}
