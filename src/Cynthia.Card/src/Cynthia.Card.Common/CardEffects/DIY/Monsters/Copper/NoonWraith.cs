using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70185")]//地灵
    public class NoonWraith : CardEffect, IHandlesEvent<AfterTurnOver>
    {//Deploy: Spawn a Mirror Image on both sides of the row. On turn end, repeat deploy ability and transform into a Nightwraith.
        //部署：在所在排的双方生成一个“镜像”。在回合结束
        public NoonWraith(GameCard card) : base(card) { }
        private int mystrength = 8;
        private int myhealth = 0;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //Spawn a Mirror Image on both sides of the row.
            await Game.CreateCardAtEnd(CardId.MirrorImage, PlayerIndex, Card.Status.CardRow);
            await Game.CreateCardAtEnd(CardId.MirrorImage, AnotherPlayer, Card.Status.CardRow);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {//On turn end, repeat deploy ability and transform into a Nightwraith.
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                mystrength = Card.Status.Strength;
                myhealth = Card.Status.HealthStatus;
                //On turn end, repeat deploy ability
                await Game.CreateCardAtEnd(CardId.MirrorImage, PlayerIndex, Card.Status.CardRow);
                await Game.CreateCardAtEnd(CardId.MirrorImage, AnotherPlayer, Card.Status.CardRow);
                int Power=Card.CardPoint();
                int Strength = Card.Status.Strength;
                int Change = Power - Strength;
                //and transform into a Nightwraith.

                await Card.Effect.Transform(CardId.NightWraith, Card, x =>
                {
                    x.Status.Strength = mystrength;
                    x.Status.HealthStatus = myhealth;
                });
                return;

            }
            return;
        }
    }
}