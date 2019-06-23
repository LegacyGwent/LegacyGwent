using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54025")] //蓝山精锐
    public class BlueMountainElite : CardEffect, IHandlesEvent<AfterCardMove>
    {
        //召唤所有同名牌。 自身移动时获得2点增益。
        public BlueMountainElite(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var deck = Game.PlayersDeck[PlayerIndex];
            var myId = Card.CardInfo().CardId;
            var cardsToPlay = deck.Where(x => x.CardInfo().CardId == myId);
            var list = cardsToPlay.ToList();
            if (!list.Any()) return 0;
            var position = Card.GetLocation();
            foreach (var it in list)
            {
                await it.Effect.Summon(position, it);
            }

            return 0;
        }

        private const int boostCount = 2;

        public async Task HandleEvent(AfterCardMove @event)
        {
            if (@event.Target == Card)
            {
                await Card.Effect.Boost(boostCount);
            }
        }
    }
}