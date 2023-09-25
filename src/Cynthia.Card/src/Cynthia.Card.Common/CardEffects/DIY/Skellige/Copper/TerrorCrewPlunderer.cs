using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70040")]//恐狼勇士
    public class TerrorCrewPlunderer : CardEffect, IHandlesEvent<AfterCardDiscard>
    {//对一个敌军随机单位造成3点伤害。被丢弃时，再次触发此能力，并将1张“恐狼勇士”加入牌组底部。

        public TerrorCrewPlunderer(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await DamageRandomEnemy();
            return 0;
        }

        public async Task HandleEvent(AfterCardDiscard @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            await DamageRandomEnemy();

            await Game.CreateCardAtEnd(CardId.TerrorCrewPlunderer, PlayerIndex, RowPosition.MyDeck);
        }

        private async Task DamageRandomEnemy()
        {
            var cards = Game.GetPlaceCards(AnotherPlayer);
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(4, Card);
        }
    }
}
