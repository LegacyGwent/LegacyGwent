using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44023")]//褐旗营
    public class DunBanner : CardEffect, IHandlesEvent<AfterTurnStart>
    {//回合开始时，若落后25点战力以上，则召唤此单位至随机排。
        public DunBanner(GameCard card) : base(card) { }



        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex)
            {
                return;
            }
            if (Game.GetPlayersPoint(PlayerIndex) + 25 < Game.GetPlayersPoint(AnotherPlayer))
            {
                //召唤全部
                var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                foreach (var card in cards)
                {
                    //召唤到末尾
                    await card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                }

            }

            return;

        }
    }
}