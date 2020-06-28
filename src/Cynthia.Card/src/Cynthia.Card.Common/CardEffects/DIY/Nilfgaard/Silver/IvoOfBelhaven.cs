using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70026")]//贝哈文的伊沃
    public class IvoOfBelhaven : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardDeath>
    {//回合开始时，若我方总战力大于对方，强化自身2点。遗愿：随机将卡组里一张稀有度最高的猎魔人单位移至卡组顶端。

        public IvoOfBelhaven(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            // await Card.Effect.Armor(2, Card);
            await Task.CompletedTask;
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if (!(Game.GetPlayersPoint(PlayerIndex) > Game.GetPlayersPoint(AnotherPlayer)))
            {
                return;
            }
            await Card.Effect.Strengthen(2, Card);
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            var deck = Game.PlayersDeck[PlayerIndex]
                .Where(card => card.Status.Categories.Contains(Categorie.Witcher))
                .OrderByDescending(x => x.Status.Group).ToList();

            if (deck.Count() == 0)
            {
                return;
            }

            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), deck.First());
        }
    }
}
