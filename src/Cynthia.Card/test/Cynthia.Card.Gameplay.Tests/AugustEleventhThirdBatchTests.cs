using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustEleventhThirdBatchTests
    {
        [Fact]
        public async Task NorthernRealmsDraugReusesDraugEffectAndArtBehavior()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersCemetery[fixture.Game.Player1Index].Clear();
            var draug = fixture.AddCard(
                fixture.Game.Player1Index, CardId.NorthernRealmsDraug, RowPosition.MyRow1);
            var firstCorpse = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var secondCorpse = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ArachasHatchling, RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            Assert.IsType<NorthernRealmsDraug>(draug.Effect);
            Assert.IsAssignableFrom<Draug>(draug.Effect);
            await fixture.Game.AddTask(async () =>
                await draug.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            var resurrected = fixture.Game.PlayersPlace[fixture.Game.Player1Index][0]
                .Where(card => card != draug)
                .ToArray();
            Assert.Equal(2, resurrected.Length);
            Assert.Contains(firstCorpse, resurrected);
            Assert.Contains(secondCorpse, resurrected);
            Assert.All(resurrected, card =>
            {
                Assert.Equal(CardId.Draugir, card.Status.CardId);
                Assert.Equal(1, card.Status.Strength);
                Assert.Equal(RowPosition.MyRow1, card.Status.CardRow);
            });
            Assert.Empty(fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
        }

        [Fact]
        public async Task ArchgriffinClearsOwnHazardAndMovesChosenEnemyCopperToOwnCemetery()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersCemetery[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersCemetery[fixture.Game.Player2Index].Clear();
            var archgriffin = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Archgriffin, RowPosition.MyRow1);
            var enemyCopper = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyCemetery);
            enemyCopper.Status.Group = Group.Copper;
            var enemySilver = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyCemetery);
            enemySilver.Status.Group = Group.Silver;
            fixture.FirstPlayer.QueueMenuCardIds(CardId.Wolf);
            await fixture.Game.GameRowEffect[fixture.Game.Player1Index][0]
                .SetStatus<BitingFrostStatus>();
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await archgriffin.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowStatus.None,
                fixture.Game.GameRowEffect[fixture.Game.Player1Index][0].RowStatus);
            Assert.Contains(enemyCopper, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.DoesNotContain(enemyCopper, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
            Assert.Contains(enemySilver, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }
    }
}
