using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54026")] //玛哈坎志愿军
    public class MahakamVolunteers : CardEffect
    {
        //召唤所有同名牌。
        public MahakamVolunteers(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
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
    }
}