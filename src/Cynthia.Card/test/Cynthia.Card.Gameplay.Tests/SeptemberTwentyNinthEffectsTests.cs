using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberTwentyNinthEffectsTests
    {
        [Fact]
        public async Task ServantBoostsAndRevealsWhenConcealedOnOwnTurn()
        {
            var f = new HeadlessGameFixture();
            var servant = f.AddCard(f.Game.Player1Index, CardId.VanMoorlehemServant, RowPosition.MyHand);
            servant.Status.IsReveal = true;
            var enemySource = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1);
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();

            await servant.Effect.Conceal(enemySource);

            Assert.True(servant.Status.IsReveal);
            Assert.Equal(4, servant.Status.HealthStatus);
        }

        [Fact]
        public async Task ServantStaysConcealedWithoutBoostOnOpponentsTurn()
        {
            var f = new HeadlessGameFixture();
            var servant = f.AddCard(f.Game.Player1Index, CardId.VanMoorlehemServant, RowPosition.MyHand);
            servant.Status.IsReveal = true;
            f.Game.GameRound = (TwoPlayer)f.Game.Player2Index;
            await f.SynchronizeClientsAsync();

            await servant.Effect.Conceal(servant);

            Assert.False(servant.Status.IsReveal);
            Assert.Equal(0, servant.Status.HealthStatus);
        }

        [Fact]
        public async Task ServantKeepsImmunityOnDeployment()
        {
            var f = new HeadlessGameFixture();
            var servant = f.AddCard(f.Game.Player1Index, CardId.VanMoorlehemServant, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await servant.Effect.CardPlayEffect(false, false);

            Assert.True(servant.Status.IsImmue);
        }
    }
}
