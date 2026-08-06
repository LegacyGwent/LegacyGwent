using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustSeventhHotfixTests
    {
        [Fact]
        public async Task IrisCompanionsLetsPlayerChooseDiscardWhileUnlockedIrisShadeIsOnBoard()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var companions = fixture.AddCard(
                fixture.Game.Player1Index, CardId.IrisCompanions, RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player1Index, "70154", RowPosition.MyRow2);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await companions.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Single(fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Equal(
                3,
                fixture.FirstPlayer.LastMenuOptionCount);
        }

        [Fact]
        public async Task LockedIrisShadeDoesNotEnableChosenDiscard()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var companions = fixture.AddCard(
                fixture.Game.Player1Index, CardId.IrisCompanions, RowPosition.MyRow1);
            var shade = fixture.AddCard(
                fixture.Game.Player1Index, "70154", RowPosition.MyRow2);
            shade.Status.IsLock = true;
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await companions.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(
                1,
                fixture.FirstPlayer.LastMenuOptionCount);
        }
    }
}
