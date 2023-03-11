using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70080")]//冷酷无情的克努特 KnuttheCallous
    public class KnuttheCallous : CardEffect
    {//Deploy: discard a card from your hand, then generate a bronze unit from your starting deck, strengthen it by 1 and add it to your hand
        public KnuttheCallous(GameCard card) : base(card) { }
        public bool IsUse { get; set; } = false;       
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {    
            if (IsUse)
            {
                return 0;
            }
            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            //discard a card from your hand
            var cards = await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersHandCard[PlayerIndex]);
            if (!cards.TrySingle(out var target))
            {
                return 0;
            }
            var targetGroup = target.Status.Group;
            if (!Game.PlayerBaseDeck[PlayerIndex].Deck.Where(x => x.Group == targetGroup).Select(x => x.CardId).TryMessOne(out var targetId, Game.RNG))
            {
                return 0;
            }
            await target.Effect.Discard(Card);

            //then generate a bronze unit from your starting deck and add it to your hand
            var list = Game.PlayerBaseDeck[PlayerIndex].Deck.Where(x => x.Is(Group.Copper, CardType.Unit));
            var selectList = list.Select(x => new CardStatus(x.CardId)).ToList();
            var result = (await Game.GetSelectMenuCards(PlayerIndex, selectList, isCanOver: false, title: "选择一张牌"));
            if (!(result).TrySingle(out var targetIndex))
            {
                return 0;
            }
            var id = selectList[targetIndex].CardId;
            var createCard = await Game.CreateCardAtEnd(id, PlayerIndex, RowPosition.MyHand);

            //strengthen it by 1
            await createCard.Effect.Strengthen(1, Card);

            return 0;
        }
    }
}