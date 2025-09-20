using System.Threading.Tasks;
using System.Linq;

namespace Cynthia.Card
{
    [CardEffectId("70187")]//地灵
    public class NightWraith : CardEffect, IHandlesEvent<AfterTurnOver>
    {//On turn end, Boost all Mirror Image copies on both sides of the row by 1 and transform self into a Noonwraith.
        //在回合结束时，提升所在排双方所有“镜像”1点增益，并变形为“老鼠”。
        public NightWraith(GameCard card) : base(card) { }
        private int mystrength = 8;
        private int myhealth = 0;
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                mystrength = Card.Status.Strength;
                myhealth = Card.Status.HealthStatus;
                //Boost all Mirror Image copies on both sides of the row by 1
                var cards = Game.RowToList(AnotherPlayer, Card.Status.CardRow).IgnoreConcealAndDead().Concat(Game.RowToList(PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead()).Where(x => x.Status.CardRow.IsOnPlace() && x.Status.CardId == CardId.MirrorImage);
                foreach (var card in cards)
                {
                    await card.Effect.Boost(1, Card);
                }
                //transform self into a Noonwraith.
                await Card.Effect.Transform(CardId.NoonWraith, Card, x =>
                {
                    x.Status.Strength = mystrength;
                    x.Status.HealthStatus = myhealth;
                });
                return;
            }
        }
    }
}