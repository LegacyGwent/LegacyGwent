using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberTwentyFourthPlumardTests
    {
        [Theory]
        [InlineData(RowPosition.MyRow1)]
        [InlineData(RowPosition.MyHand)]
        [InlineData(RowPosition.MyDeck)]
        public async Task AlliedVampireKillsBoostPlumardInAllActiveZones(RowPosition zone)
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, zone);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow2);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await victim.Effect.Damage(1, vampire));
            Assert.Equal(1, bird.Status.HealthStatus);
            Assert.False(victim.IsAliveOnPlance());
        }

        [Fact]
        public async Task DrainKillOfDoomedUnitCountsOnceAndSameNameDoesNotChain()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyRow1);
            var otherBird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyRow1);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            victim.Status.IsDoomed = true;
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await vampire.Effect.Drain(1, victim));
            Assert.Equal(1, bird.Status.HealthStatus);
            Assert.Equal(1, otherBird.Status.HealthStatus);
            Assert.Equal(RowPosition.Banish, victim.Status.CardRow);
        }

        [Fact]
        public async Task DifferentNameRowDrainIsCopiedOncePerBirdWithoutRecursion()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyRow1);
            var otherBird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyRow1);
            var offRow = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyRow2);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await vampire.Effect.Drain(2, victim));
            Assert.Equal(-4, victim.Status.HealthStatus);
            Assert.Equal(1, bird.Status.HealthStatus);
            Assert.Equal(1, otherBird.Status.HealthStatus);
            Assert.Equal(0, offRow.Status.HealthStatus);
            Assert.Equal(2, vampire.Status.HealthStatus);
        }

        [Fact]
        public async Task NonVampireEnemyNonlethalBanishAndDiscardDoNotGrantKillBoost()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var wolf = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            var enemyVampire = f.AddCard(f.Game.Player2Index, CardId.Alp, RowPosition.MyRow1);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var killedByWolf = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            var killedByEnemy = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 1);
            var survivor = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 5);
            var banished = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 2);
            var discarded = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, 2);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => {
                await killedByWolf.Effect.Damage(1, wolf);
                await killedByEnemy.Effect.Damage(1, enemyVampire);
                await survivor.Effect.Damage(1, vampire);
                await banished.Effect.Banish();
                await discarded.Effect.Discard(vampire);
            });
            Assert.Equal(0, bird.Status.HealthStatus);
        }

        [Fact]
        public async Task SourceAttributedDestroyAndConsumeUseTheSameKillContract()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var destroyed = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 3);
            var consumed = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 2);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => {
                await destroyed.Effect.ToCemetery(source: vampire);
                await vampire.Effect.Consume(consumed);
            });
            Assert.Equal(2, bird.Status.HealthStatus);
        }

        [Fact]
        public async Task TheSamePhysicalUnitCanBeDestroyedAgainAfterResurrection()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.TemerianInfantryman, RowPosition.MyRow1, 1);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await victim.Effect.Damage(1, vampire));
            Assert.Equal(1, bird.Status.HealthStatus);
            await f.Game.AddTask(async () => await victim.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), vampire));
            Assert.True(victim.IsAliveOnPlance());
            await f.Game.AddTask(async () => await victim.Effect.Damage(1, vampire));
            Assert.Equal(2, bird.Status.HealthStatus);
        }

        [Fact]
        public async Task QueuedDeathKeepsVampireAttributionAfterItsSourceTransforms()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => {
                await victim.Effect.Damage(1, vampire);
                await vampire.Effect.Transform(CardId.Wolf, vampire);
            });
            Assert.False(vampire.HasAllCategorie(Categorie.Vampire));
            Assert.Equal(1, bird.Status.HealthStatus);
        }

        [Fact]
        public async Task GolyatReactionDeathDoesNotBecomeTheOriginalVampiresKill()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, "23003", RowPosition.MyRow1, 3);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await victim.Effect.Damage(1, vampire));
            Assert.Equal(0, bird.Status.HealthStatus);
            Assert.False(victim.IsAliveOnPlance());
        }

        [Fact]
        public async Task RegisConsumesLiveDeckUnitButCemeteryConsumeIsNotAnotherKill()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player2Index].Clear();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var regis = f.AddCard(f.Game.Player1Index, CardId.RegisHigherVampire, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Alp, RowPosition.MyDeck);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await regis.Effects.RaiseEvent(new CardPlayEffect(false, false)));
            Assert.Equal(1, bird.Status.HealthStatus);
            Assert.Equal(RowPosition.MyCemetery, victim.Status.CardRow);
            await f.Game.AddTask(async () => await regis.Effect.Consume(victim));
            Assert.Equal(1, bird.Status.HealthStatus);
            Assert.Equal(RowPosition.Banish, victim.Status.CardRow);
        }

        [Theory]
        [InlineData(RowPosition.MyHand)]
        [InlineData(RowPosition.MyDeck)]
        public async Task ConsumeLiveOffBoardUnitCountsAsDestruction(RowPosition zone)
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Alp, zone);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await vampire.Effect.Consume(victim));
            Assert.Equal(1, bird.Status.HealthStatus);
        }

        [Fact]
        public async Task DetlaffDelayedDestroyRetainsItsVampireAttribution()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var detlaff = f.AddCard(f.Game.Player1Index, "70002", RowPosition.MyHand);
            var victim = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyDeck, 1);
            await f.SynchronizeClientsAsync();
            await f.Game.RoundPlayCard(f.Game.Player1Index, new RoundInfo {
                HandCardIndex = f.Game.PlayersHandCard[f.Game.Player1Index].IndexOf(detlaff),
                CardLocation = new CardLocation(RowPosition.MyRow1, 0)
            });
            Assert.True(victim.IsAliveOnPlance());
            await f.Game.AddTask(async () => await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index)));
            Assert.Equal(1, bird.Status.HealthStatus);
            Assert.False(victim.IsAliveOnPlance());
        }

        [Fact]
        public async Task AttributedDamageStillHonorsExistingRemovalOverrides()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            var observer = new RemovalObserver(victim);
            HeadlessGameFixture.ReplaceMainEffect(victim, observer);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await victim.Effect.Damage(1, vampire));
            Assert.Equal(1, observer.Removals);
            Assert.Equal(1, bird.Status.HealthStatus);
        }

        private sealed class RemovalObserver : CardEffect
        {
            public RemovalObserver(GameCard card) : base(card) { }
            public int Removals { get; private set; }
            public override Task ToCemetery(CardBreakEffectType type = CardBreakEffectType.ToCemetery,
                bool isNeedBanish = true, bool isNeedSentEvent = true)
            {
                Removals++;
                return base.ToCemetery(type, isNeedBanish, isNeedSentEvent);
            }
        }

        [Fact]
        public async Task ShieldAndRoundCleanupAreNotKills()
        {
            var f = new HeadlessGameFixture();
            var bird = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            victim.Status.IsShield = true;
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await victim.Effect.Damage(10, vampire));
            Assert.Equal(0, bird.Status.HealthStatus);
            await f.Game.AddTask(async () => await victim.Effect.RoundEnd());
            Assert.Equal(0, bird.Status.HealthStatus);
        }

        [Fact]
        public async Task LockedAndCemeteryPlumardsDoNotRespondAndWeakenDeathCounts()
        {
            var f = new HeadlessGameFixture();
            var active = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyDeck);
            var locked = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyHand);
            locked.Status.IsLock = true;
            var grave = f.AddCard(f.Game.Player1Index, CardId.Plumard, RowPosition.MyCemetery);
            var vampire = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            var victim = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await victim.Effect.Weaken(1, vampire));
            Assert.Equal(1, active.Status.HealthStatus);
            Assert.Equal(0, locked.Status.HealthStatus);
            Assert.Equal(0, grave.Status.HealthStatus);
        }
    }
}
