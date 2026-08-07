using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustSixthNilfgaardBatchTests
    {
        [Fact]
        public async Task AssassinationIgnoresArmorForBothDamageInstances()
        {
            var fixture = new HeadlessGameFixture();
            var assassination = fixture.AddCard(
                fixture.Game.Player1Index, "32015", RowPosition.MyHand);
            var first = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, strength: 8);
            var second = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, strength: 8);
            first.Status.Armor = 5;
            second.Status.Armor = 5;
            await fixture.SynchronizeClientsAsync();

            await assassination.Effect.CardUseEffect();

            Assert.False(first.Status.CardRow.IsOnPlace());
            Assert.False(second.Status.CardRow.IsOnPlace());
            Assert.Equal(0, first.Status.Armor);
            Assert.Equal(0, second.Status.Armor);
        }

        [Fact]
        public async Task TreasonCanDuelTwoNonAdjacentUnitsOnTheSameRow()
        {
            var fixture = new HeadlessGameFixture();
            var treason = fixture.AddCard(
                fixture.Game.Player1Index, "33020", RowPosition.MyHand);
            var first = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            var middle = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1, strength: 5);
            middle.Status.IsImmue = true;
            var last = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, strength: 1);
            await fixture.SynchronizeClientsAsync();

            await treason.Effect.CardUseEffect();

            Assert.True(first.Status.CardRow.IsOnPlace());
            Assert.True(middle.Status.CardRow.IsOnPlace());
            Assert.True(!first.Status.CardRow.IsOnPlace() || !last.Status.CardRow.IsOnPlace(), $"first={first.Status.CardRow}, last={last.Status.CardRow}");
        }

        [Fact]
        public async Task HeftyHelgeRevealBranchHitsBoardAndRevealedNonSpyingCardZones()
        {
            var fixture = new HeadlessGameFixture();
            var helge = fixture.AddCard(
                fixture.Game.Player1Index, "33009", RowPosition.MyRow1);
            var sameRow = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var otherRow = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyRow2);
            var revealedHand = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, strength: 5);
            var hiddenHand = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, strength: 5);
            var revealedDeck = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck, strength: 5);
            var spyingDeck = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck, strength: 5);
            revealedHand.Status.IsReveal = true;
            revealedDeck.Status.IsReveal = true;
            spyingDeck.Status.IsReveal = true;
            spyingDeck.Status.IsSpying = true;
            await fixture.SynchronizeClientsAsync();

            await helge.Effects.RaiseEvent(new CardPlayEffect(false, true));

            Assert.Equal(-1, sameRow.Status.HealthStatus);
            Assert.Equal(-1, otherRow.Status.HealthStatus);
            Assert.Equal(-1, revealedHand.Status.HealthStatus);
            Assert.Equal(0, hiddenHand.Status.HealthStatus);
            Assert.Equal(-1, revealedDeck.Status.HealthStatus);
            Assert.Equal(0, spyingDeck.Status.HealthStatus);
        }

        [Fact]
        public async Task HeftyHelgeNormalBranchOnlyHitsTheOppositeRow()
        {
            var fixture = new HeadlessGameFixture();
            var helge = fixture.AddCard(
                fixture.Game.Player1Index, "33009", RowPosition.MyRow1);
            var sameRow = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var otherRow = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyRow2);
            var revealedHand = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, strength: 5);
            revealedHand.Status.IsReveal = true;
            await fixture.SynchronizeClientsAsync();

            await helge.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Equal(0, sameRow.Status.HealthStatus);
            Assert.Equal(-1, otherRow.Status.HealthStatus);
            Assert.Equal(0, revealedHand.Status.HealthStatus);
        }

        [Fact]
        public async Task AlbaPikemanGainsArmorAndSummonsOneCopyAtEachOwnerTurnStart()
        {
            var fixture = new HeadlessGameFixture();
            var pikeman = fixture.AddCard(
                fixture.Game.Player1Index, "34024", RowPosition.MyRow1);
            var firstCopy = fixture.AddCard(
                fixture.Game.Player1Index, "34024", RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player1Index, "34024", RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await pikeman.Effect.CardPlayEffect(false, false);
            await pikeman.Effect.CardDownEffect(false, false);
            Assert.Equal(2, pikeman.Status.Armor);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.True(firstCopy.Status.CardRow.IsInDeck());

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Single(fixture.Game.PlayersDeck[fixture.Game.Player1Index],
                x => x.Status.CardId == "34024");
            Assert.Equal(2, fixture.Game.GetAllCard(fixture.Game.Player1Index)
                .Single(x => x != pikeman && x.Status.CardId == "34024" && x.Status.CardRow.IsOnPlace()).Status.Armor);
        }

        [Fact]
        public async Task NilfgaardianKnightRevealsTheFirstLowestRarityCardDeterministically()
        {
            var fixture = new HeadlessGameFixture();
            var knight = fixture.AddCard(
                fixture.Game.Player1Index, "34004", RowPosition.MyRow1);
            var silver = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyHand);
            var firstCopper = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            var secondCopper = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await knight.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.True(firstCopper.Status.IsReveal);
            Assert.False(secondCopper.Status.IsReveal);
            Assert.False(silver.Status.IsReveal);
            Assert.Equal(2, knight.Status.Armor);
        }

        [Fact]
        public async Task CupbearerConcealsAndBoostsOneBronzeHandCardOnOwnerTurnStart()
        {
            var fixture = new HeadlessGameFixture();
            var cupbearer = fixture.AddCard(
                fixture.Game.Player1Index, CardId.VanMoorlehemsCupbearer, RowPosition.MyRow1);
            var hand = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            hand.Status.IsReveal = true;
            await fixture.SynchronizeClientsAsync();

            await cupbearer.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.True(hand.Status.IsReveal);
            Assert.Equal(0, hand.Status.HealthStatus);

            await cupbearer.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.False(hand.Status.IsReveal);
            Assert.Equal(1, hand.Status.HealthStatus);
        }

        [Fact]
        public async Task CupbearerDoesNotBoostAnUnrevealedBronzeHandCard()
        {
            var fixture = new HeadlessGameFixture();
            var cupbearer = fixture.AddCard(
                fixture.Game.Player1Index, CardId.VanMoorlehemsCupbearer, RowPosition.MyRow1);
            var hand = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await cupbearer.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));

            Assert.False(hand.Status.IsReveal);
            Assert.Equal(0, hand.Status.HealthStatus);
        }

        [Fact]
        public async Task MageInfiltratorCanCopyBoardOrRevealedHandAsDoomedAndEndsWithoutTargets()
        {
            var boardFixture = new HeadlessGameFixture();
            var boardInfiltrator = boardFixture.AddCard(
                boardFixture.Game.Player2Index, CardId.MageInfiltrator, RowPosition.MyRow1);
            boardFixture.AddCard(
                boardFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2);
            boardFixture.AddCard(
                boardFixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            await boardFixture.SynchronizeClientsAsync();

            await boardInfiltrator.Effects.RaiseEvent(new CardPlayEffect(true, false));

            var boardCopy = Assert.Single(boardFixture.Game.PlayersStay[boardFixture.Game.Player1Index]);
            Assert.Equal(CardId.Wolf, boardCopy.Status.CardId);
            Assert.True(boardCopy.Status.IsDoomed);

            var handFixture = new HeadlessGameFixture();
            var handInfiltrator = handFixture.AddCard(
                handFixture.Game.Player2Index, CardId.MageInfiltrator, RowPosition.MyRow1);
            var revealed = handFixture.AddCard(
                handFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyHand);
            revealed.Status.IsReveal = true;
            await handFixture.SynchronizeClientsAsync();

            await handInfiltrator.Effects.RaiseEvent(new CardPlayEffect(true, false));

            var handCopy = Assert.Single(handFixture.Game.PlayersStay[handFixture.Game.Player1Index]);
            Assert.Equal(CardId.Wolf, handCopy.Status.CardId);
            Assert.True(handCopy.Status.IsDoomed);

            var emptyFixture = new HeadlessGameFixture();
            var emptyInfiltrator = emptyFixture.AddCard(
                emptyFixture.Game.Player2Index, CardId.MageInfiltrator, RowPosition.MyRow1);
            await emptyFixture.SynchronizeClientsAsync();

            await emptyInfiltrator.Effects.RaiseEvent(new CardPlayEffect(true, false));

            Assert.Empty(emptyFixture.Game.PlayersStay[emptyFixture.Game.Player1Index]);
        }

        [Fact]
        public async Task OddHalfValuesAlwaysRoundUpForRepresentativeEffects()
        {
            var spotterFixture = new HeadlessGameFixture();
            var spotter = spotterFixture.AddCard(
                spotterFixture.Game.Player1Index, "34021", RowPosition.MyRow1);
            var revealed = spotterFixture.AddCard(
                spotterFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, strength: 7);
            revealed.Status.IsReveal = true;
            await spotterFixture.SynchronizeClientsAsync();

            await spotter.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(4, spotter.Status.HealthStatus);

            var vincentFixture = new HeadlessGameFixture();
            var vincent = vincentFixture.AddCard(
                vincentFixture.Game.Player1Index, "43008", RowPosition.MyRow1);
            var armored = vincentFixture.AddCard(
                vincentFixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2);
            armored.Status.Armor = 5;
            await vincentFixture.SynchronizeClientsAsync();

            await vincent.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(3, vincent.Status.HealthStatus);
            Assert.Equal(0, armored.Status.Armor);
        }

        [Fact]
        public async Task XarthisiusMovesTheChosenDeckCardToBottomAndTogglesLock()
        {
            var fixture = new HeadlessGameFixture();
            var xarthisius = fixture.AddCard(
                fixture.Game.Player1Index, "32002", RowPosition.MyRow1);
            var first = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await xarthisius.Effects.RaiseEvent(new CardPlayEffect(false, false));

            var moved = fixture.Game.PlayersDeck[fixture.Game.Player2Index].Last();
            Assert.True(moved.Status.IsLock, $"first={first.Status.IsLock}, moved={moved.Status.CardId}:{moved.Status.IsLock}");
        }
    }
}
