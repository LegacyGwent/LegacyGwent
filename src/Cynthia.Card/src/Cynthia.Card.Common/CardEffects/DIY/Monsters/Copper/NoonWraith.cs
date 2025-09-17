using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70185")]//地灵
    public class NoonWraith : CardEffect, IHandlesEvent<AfterTurnOver>
    {//Deploy: Spawn a Mirror Image on both sides of the row. On turn end, repeat deploy ability and transform into a Nightwraith.
        //部署：在所在排的双方生成一个“镜像”。在回合结束
        public NoonWraith(GameCard card) : base(card) { }
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
                //On turn end, repeat deploy ability
                await Game.CreateCardAtEnd(CardId.MirrorImage, PlayerIndex, Card.Status.CardRow);
                await Game.CreateCardAtEnd(CardId.MirrorImage, AnotherPlayer, Card.Status.CardRow);
                int Power=Card.CardPoint();
                int Strength = Card.Status.Strength;
                int Change = Power - Strength;
                //and transform into a Nightwraith.

                await Card.Effect.Transform(CardId.NightWraith, Card, x => x.Status.Strength = Strength, isForce: true);
                if (Change>0)
                {
                    await Boost_Quiet(Change, Card);
                }
                if (Change<0)
                {
                    await Lower_Power_By(-Change, Card);
                }
                return;

            }
            return;
        }
    }
}