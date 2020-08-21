using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70016")]//苏克鲁斯
    public class Sukrus : CardEffect
    {//选择手牌中的一张铜卡，丢弃所有牌组中该牌的同名牌。
        public Sukrus(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var unitHandCard = Game.PlayersHandCard[PlayerIndex]
                .Where(x =>
                    // x.Status.Type == CardType.Unit &&
                    x.IsAnyGroup(Group.Copper)
                )
                .ToList();
            if (unitHandCard.Count() == 0)
            {
                return 0;
            }

            var targetcards = await Game.GetSelectMenuCards(PlayerIndex, unitHandCard, isCanOver: false);
            if (!targetcards.TrySingle(out var target))
            {
                return 0;
            }

            var result = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == target.Status.CardId).ToList();

            foreach (var card in result)
            {
                await card.Effect.Discard(Card);
            }
            return 0;
        }
    }
}