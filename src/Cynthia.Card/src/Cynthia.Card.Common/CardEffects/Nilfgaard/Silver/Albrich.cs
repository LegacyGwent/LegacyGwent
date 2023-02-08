using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33006")]//亚伯力奇
    public class Albrich : CardEffect
    {//力竭。休战：双方各抽1张牌。对方抽到的牌将被揭示。

        public Albrich(GameCard card) : base(card) { }
        public bool IsUse { get; set; } = false;
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (Game.IsPlayersPass[Game.AnotherPlayer(Card.PlayerIndex)]) return 0;
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            var cards = await Game.DrawCard(1, 1);
            foreach (var item in cards.Item1.Concat(cards.Item2))
            {
                if (item.PlayerIndex != PlayerIndex)
                    await item.Effect.Reveal(Card);
            }
            return 0;
        }
    }
}