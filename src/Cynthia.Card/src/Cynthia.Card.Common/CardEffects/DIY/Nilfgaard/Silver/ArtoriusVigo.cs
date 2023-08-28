using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70103")]//亚托列司·薇歌 ArtoriusVigo
    public class ArtoriusVigo : CardEffect
    {//选择1张被揭示的手牌，并将其变形为1张己方起始牌组中铜色单位牌的指定原始同名牌，随后将其揭示。
        public ArtoriusVigo(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //丢弃1张手牌
            var cards = await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersHandCard[PlayerIndex].Where(x => x.Status.IsReveal).ToList());
            if (!cards.TrySingle(out var target))
            {
                return 0;
            }
            var targetGroup = target.Status.Group;
            if (!Game.PlayerBaseDeck[PlayerIndex].Deck.Where(x => x.Group == targetGroup).Select(x => x.CardId).TryMessOne(out var targetId, Game.RNG))
            {
                return 0;
            }

            //并在手牌中添加1张己方起始牌组中铜色单位牌的指定原始同名牌
            var list = Game.PlayerBaseDeck[PlayerIndex].Deck.Where(x => x.Is(Group.Copper, CardType.Unit));
            var selectList = list.Select(x => new CardStatus(x.CardId)).ToList();
            var result = (await Game.GetSelectMenuCards(PlayerIndex, selectList, isCanOver: false, title: "选择一张牌"));
            if (!(result).TrySingle(out var targetIndex))
            {
                return 0;
            }
            var id = selectList[targetIndex].CardId;
            await target.Effect.Transform(id, Card, null, true);
            // await target.Effect.Conceal(Card);
            //随后将其揭示
            await target.Effect.Reveal(Card);

            return 0;
        }
    }
}
