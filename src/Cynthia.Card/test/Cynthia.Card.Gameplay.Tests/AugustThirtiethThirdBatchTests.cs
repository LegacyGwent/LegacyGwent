using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustThirtiethThirdBatchTests
    {
        [Fact]
        public async Task DandelionVaingloryCountsBothNewZoltanCardsInTheStartingDeck()
        {
            var fixture = new HeadlessGameFixture();
            var dandelion = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.DandelionVainglory,
                RowPosition.MyRow1);
            fixture.Game.PlayerBaseDeck[fixture.Game.Player1Index].Deck = new List<GwentCard>
            {
                GwentMap.CardMap[CardId.ZoltanWarrior],
                GwentMap.CardMap[CardId.ZoltansCompany],
                GwentMap.CardMap[CardId.Wolf]
            };
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => dandelion.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(6, dandelion.Status.HealthStatus);
        }

        [Fact]
        public async Task DandelionVaingloryDoesNotCountUnrelatedStartingDeckCards()
        {
            var fixture = new HeadlessGameFixture();
            var dandelion = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.DandelionVainglory,
                RowPosition.MyRow1);
            fixture.Game.PlayerBaseDeck[fixture.Game.Player1Index].Deck = new List<GwentCard>
            {
                GwentMap.CardMap[CardId.DwarfMiner],
                GwentMap.CardMap[CardId.Wolf]
            };
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => dandelion.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(0, dandelion.Status.HealthStatus);
        }
    }
}
