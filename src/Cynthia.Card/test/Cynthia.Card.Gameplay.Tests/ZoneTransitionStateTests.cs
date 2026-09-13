using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class ZoneTransitionStateTests
    {
        [Theory]
        [InlineData(4)]
        [InlineData(-3)]
        public async Task ToCemeteryClearsTemporaryStateButPreservesLockBasePowerAndCountdown(int health)
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyRow1, health);
            var effect = card.Effect;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.ToCemetery());

            Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow);
            AssertRepaired(card, locked: true);
            Assert.Same(effect, card.Effect);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void RepairOnlyClearsLockWhenExplicitlyRequested(bool unlock)
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyHand);
            card.Status.IsDoomed = true;

            card.Effect.Repair(unlock);

            AssertRepaired(card, locked: !unlock, doomed: true);
            Assert.Equal(RowPosition.MyHand, card.Status.CardRow);
        }

        [Fact]
        public async Task EnteringCemeteryWithDoomedBanishesInsteadOfKeepingAReusableGraveyardCard()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyRow1);
            card.Status.IsDoomed = true;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.ToCemetery());

            Assert.Equal(RowPosition.Banish, card.Status.CardRow);
            Assert.DoesNotContain(card, f.Game.PlayersCemetery[f.Game.Player1Index]);
            AssertRepaired(card, locked: true, doomed: true);
        }

        [Fact]
        public async Task NativeCemeterySummonPreservesLockAndOtherStateButClearsReveal()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyCemetery);
            card.Status.IsDoomed = true;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.Summon(new CardLocation(RowPosition.MyRow1, 0), card));

            Assert.Equal(RowPosition.MyRow1, card.Status.CardRow);
            AssertMarked(card, locked: true, revealed: false, doomed: true);
        }

        [Theory]
        [InlineData(RowPosition.MyRow1)]
        [InlineData(RowPosition.MyDeck)]
        [InlineData(RowPosition.MyStay)]
        public async Task NativeResurrectionUnlocksWithoutRepairingOtherStateRegardlessOfDestination(RowPosition destination)
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyCemetery);
            card.Status.IsDoomed = true;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.Resurrect(new CardLocation(destination, 0), card));

            Assert.Equal(destination, card.Status.CardRow);
            AssertMarked(card, locked: false, revealed: true, doomed: true);
        }

        [Fact]
        public async Task NativeResurrectionUnlocksBeforeRejectingAFullDestinationRow()
        {
            var f = new HeadlessGameFixture();
            f.Game.RowMaxCount = 1;
            var card = AddMarkedCard(f, RowPosition.MyCemetery);
            f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.Resurrect(new CardLocation(RowPosition.MyRow1, 0), card));

            Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow);
            AssertMarked(card, locked: false, revealed: true);
        }

        [Fact]
        public async Task DirectPlayFromBoardDoesNotProvideTheExplicitRepairUsedByReplayCards()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyRow1);
            // A concealed unit takes Play's separate ambush path; this is normal unit play.
            card.Status.Conceal = false;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.Play(new CardLocation(RowPosition.MyRow2, 0), isFromHand: false));

            Assert.Equal(RowPosition.MyRow2, card.Status.CardRow);
            AssertMarked(card, locked: true, revealed: false, concealed: false);
        }

        [Fact]
        public async Task DecoyExplicitlyRepairsAndUnlocksThenAddsItsOwnBoostBeforeReplay()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyRow1);
            card.Status.Group = Group.Copper;
            card.Status.IsImmue = false;
            card.Status.Conceal = false;
            card.Status.IsDoomed = true;
            var decoy = f.AddCard(f.Game.Player1Index, CardId.Decoy, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await decoy.Effect.CardUse();

            Assert.True(card.Status.CardRow.IsOnPlace());
            AssertRepaired(card, locked: false, doomed: true, health: 3);
            Assert.Empty(f.Game.PlayersStay[f.Game.Player1Index]);
        }

        [Theory]
        [InlineData(RowPosition.MyRow1, RowPosition.MyDeck)]
        [InlineData(RowPosition.MyCemetery, RowPosition.MyDeck)]
        [InlineData(RowPosition.MyHand, RowPosition.MyDeck)]
        [InlineData(RowPosition.MyRow1, RowPosition.MyHand)]
        [InlineData(RowPosition.MyRow1, RowPosition.MyCemetery)]
        public async Task RawShowCardMoveChangesTheZoneWithoutRepairOrUnlock(RowPosition from, RowPosition to)
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, from);
            card.Status.IsDoomed = true;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => f.Game.ShowCardMove(new CardLocation(to, 0), card));

            Assert.Equal(to, card.Status.CardRow);
            AssertMarked(card, locked: true, revealed: true, doomed: true);
        }

        [Fact]
        public async Task NativeBoardMovePreservesStateWhenRemainingOnTheSameSide()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), card));

            Assert.Equal(RowPosition.MyRow2, card.Status.CardRow);
            AssertMarked(card, locked: true, revealed: true);
        }

        [Fact]
        public async Task NennekeExplicitlyRepairsCemeteryCardsWithoutUnlockingThem()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyCemetery);
            card.Status.Group = Group.Copper;
            var nenneke = f.AddCard(f.Game.Player1Index, CardId.Nenneke, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => nenneke.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowPosition.MyDeck, card.Status.CardRow);
            AssertRepaired(card, locked: true);
        }

        [Fact]
        public async Task PrincessPavettaExplicitlyRepairsAndUnlocksTheBoardCardReturnedToDeck()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyRow1);
            card.Status.Group = Group.Copper;
            card.Status.IsImmue = false;
            card.Status.Conceal = false;
            var pavetta = f.AddCard(f.Game.Player1Index, CardId.PrincessPavetta, RowPosition.MyRow2, 30);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => pavetta.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowPosition.MyDeck, card.Status.CardRow);
            AssertRepaired(card, locked: false);
        }

        [Fact]
        public async Task NativeHandSwapOnlyClearsRevealAndRetainsLockAndPowerChanges()
        {
            var f = new HeadlessGameFixture();
            var card = AddMarkedCard(f, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => card.Effect.Swap());

            Assert.Equal(RowPosition.MyDeck, card.Status.CardRow);
            AssertMarked(card, locked: true, revealed: false);
        }

        [Fact]
        public async Task StrengthenAndWeakenChangesSurviveOrdinaryDeath()
        {
            var f = new HeadlessGameFixture();
            var card = f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 12);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
            {
                await card.Effect.Strengthen(4, card);
                await card.Effect.Weaken(2, card);
                await card.Effect.Boost(3, card);
                await card.Effect.ToCemetery();
            });

            Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow);
            Assert.Equal(14, card.Status.Strength);
            Assert.Equal(0, card.Status.HealthStatus);
        }

        // These deliberately marked states distinguish what each native helper
        // changes. Ordinary cemetery entry has already cleared many of them.
        private static GameCard AddMarkedCard(HeadlessGameFixture f, RowPosition row, int health = -3)
        {
            var card = f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, row, 12);
            card.Status.HealthStatus = health;
            card.Status.Armor = 6;
            card.Status.IsLock = true;
            card.Status.IsShield = true;
            card.Status.IsResilience = true;
            card.Status.IsSpying = true;
            card.Status.Conceal = true;
            card.Status.IsReveal = true;
            card.Status.IsImmue = true;
            card.Status.IsCardBack = true;
            card.Status.Countdown = 7;
            card.Status.IsCountdown = true;
            return card;
        }

        private static void AssertMarked(GameCard card, bool locked, bool revealed,
            bool doomed = false, bool concealed = true)
        {
            Assert.Equal(12, card.Status.Strength);
            Assert.Equal(-3, card.Status.HealthStatus);
            Assert.Equal(6, card.Status.Armor);
            Assert.Equal(locked, card.Status.IsLock);
            Assert.True(card.Status.IsShield);
            Assert.True(card.Status.IsResilience);
            Assert.True(card.Status.IsSpying);
            Assert.Equal(concealed, card.Status.Conceal);
            Assert.Equal(revealed, card.Status.IsReveal);
            Assert.True(card.Status.IsImmue);
            Assert.True(card.Status.IsCardBack);
            Assert.Equal(doomed, card.Status.IsDoomed);
            Assert.Equal(7, card.Status.Countdown);
            Assert.True(card.Status.IsCountdown);
        }

        private static void AssertRepaired(GameCard card, bool locked, bool doomed = false, int health = 0)
        {
            Assert.Equal(12, card.Status.Strength);
            Assert.Equal(health, card.Status.HealthStatus);
            Assert.Equal(0, card.Status.Armor);
            Assert.Equal(locked, card.Status.IsLock);
            Assert.False(card.Status.IsShield);
            Assert.False(card.Status.IsResilience);
            Assert.False(card.Status.IsSpying);
            Assert.False(card.Status.Conceal);
            Assert.False(card.Status.IsReveal);
            Assert.False(card.Status.IsImmue);
            Assert.False(card.Status.IsCardBack);
            Assert.Equal(doomed, card.Status.IsDoomed);
            Assert.Equal(7, card.Status.Countdown);
            Assert.True(card.Status.IsCountdown);
        }
    }
}
