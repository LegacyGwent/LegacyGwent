using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustThirtiethFirstBatchTests
    {
        [Fact]
        public async Task EgmondRemovesBoostAndDealsTheSameDamage()
        {
            var fixture = new HeadlessGameFixture();
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            ally.Status.HealthStatus = 4;
            var egmond = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => egmond.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(0, ally.Status.HealthStatus);
            Assert.Equal(-4, enemy.Status.HealthStatus);
            Assert.Equal(0, egmond.Status.HealthStatus);
        }

        [Fact]
        public async Task EgmondGetsOneBoostWhenItsDamageDestroysTheTarget()
        {
            var fixture = new HeadlessGameFixture();
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            ally.Status.HealthStatus = 5;
            var egmond = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            var firstEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 4);
            var secondEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.ArachasHatchling, RowPosition.MyRow2, 8);
            fixture.Game.GameRound = (TwoPlayer)fixture.Game.Player2Index;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => egmond.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.False(firstEnemy.Status.CardRow.IsOnPlace());
            Assert.Equal(1, egmond.Status.HealthStatus);
            Assert.Equal(0, secondEnemy.Status.HealthStatus);
            Assert.Equal(2, fixture.FirstPlayer.PlaceSelectionSources.Count);
        }

        [Fact]
        public async Task EgmondOwnTurnKillRewardQueuesOneMoreAbilityWithoutRecursiveDispatch()
        {
            var fixture = new HeadlessGameFixture();
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            ally.Status.HealthStatus = 5;
            var egmond = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            var firstEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 4);
            var secondEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.ArachasHatchling, RowPosition.MyRow2, 8);
            fixture.Game.GameRound = (TwoPlayer)fixture.Game.Player1Index;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => egmond.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.False(firstEnemy.Status.CardRow.IsOnPlace());
            Assert.Equal(0, secondEnemy.Status.HealthStatus);
            Assert.Equal(4, fixture.FirstPlayer.PlaceSelectionSources.Count);
        }

        [Fact]
        public async Task EgmondRepeatsOnlyWhenBoostedDuringItsOwnersTurn()
        {
            var ownTurn = new HeadlessGameFixture();
            var ally = ownTurn.AddCard(
                ownTurn.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            ally.Status.HealthStatus = 3;
            var egmond = ownTurn.AddCard(
                ownTurn.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            var enemy = ownTurn.AddCard(
                ownTurn.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            ownTurn.Game.GameRound = (TwoPlayer)ownTurn.Game.Player1Index;
            await ownTurn.SynchronizeClientsAsync();

            await ownTurn.Game.AddTask(() => egmond.Effect.Boost(1, ally));

            Assert.Equal(0, ally.Status.HealthStatus);
            Assert.Equal(-3, enemy.Status.HealthStatus);
            Assert.Equal(1, egmond.Status.HealthStatus);

            var enemyTurn = new HeadlessGameFixture();
            var untouchedAlly = enemyTurn.AddCard(
                enemyTurn.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            untouchedAlly.Status.HealthStatus = 3;
            var passiveEgmond = enemyTurn.AddCard(
                enemyTurn.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            var untouchedEnemy = enemyTurn.AddCard(
                enemyTurn.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            enemyTurn.Game.GameRound = (TwoPlayer)enemyTurn.Game.Player2Index;
            await enemyTurn.SynchronizeClientsAsync();

            await enemyTurn.Game.AddTask(() => passiveEgmond.Effect.Boost(1, untouchedAlly));

            Assert.Equal(3, untouchedAlly.Status.HealthStatus);
            Assert.Equal(0, untouchedEnemy.Status.HealthStatus);
            Assert.Equal(1, passiveEgmond.Status.HealthStatus);
            Assert.Empty(enemyTurn.FirstPlayer.PlaceSelectionSources);
        }

        [Fact]
        public async Task EgmondStopsCleanlyWithZeroBoostOrNoEnemyTarget()
        {
            var zeroBoost = new HeadlessGameFixture();
            var ally = zeroBoost.AddCard(
                zeroBoost.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            var egmond = zeroBoost.AddCard(
                zeroBoost.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            var enemy = zeroBoost.AddCard(
                zeroBoost.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            await zeroBoost.SynchronizeClientsAsync();

            await zeroBoost.Game.AddTask(
                () => egmond.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(0, enemy.Status.HealthStatus);
            Assert.Equal(0, egmond.Status.HealthStatus);

            var noEnemy = new HeadlessGameFixture();
            var boostedAlly = noEnemy.AddCard(
                noEnemy.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 5);
            boostedAlly.Status.HealthStatus = 3;
            var secondEgmond = noEnemy.AddCard(
                noEnemy.Game.Player1Index, CardId.Egmond, RowPosition.MyRow2);
            await noEnemy.SynchronizeClientsAsync();

            await noEnemy.Game.AddTask(
                () => secondEgmond.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(0, boostedAlly.Status.HealthStatus);
            Assert.Equal(0, secondEgmond.Status.HealthStatus);
        }

        [Fact]
        public async Task QueenAdaliaAddsEveryBronzeCintraUnitToDeckBottom()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var sentinel = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var adalia = fixture.AddCard(
                fixture.Game.Player1Index, CardId.QueenAdalia, RowPosition.MyRow1);
            var expected = GwentMap.GetCards()
                .FilterCards(
                    Group.Copper,
                    CardType.Unit,
                    card => card.HasAllCategorie(Categorie.Cintra))
                .Select(card => card.CardId)
                .ToArray();
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => adalia.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            var deck = fixture.Game.PlayersDeck[fixture.Game.Player1Index];
            Assert.Same(sentinel, deck.First());
            Assert.Equal(expected, deck.Skip(1).Select(card => card.Status.CardId));
            Assert.Equal(expected.Length, fixture.FirstPlayer.LastMenuOptionCount);
        }

        [Fact]
        public async Task AddedCountdownsRemainVisibleAndKeepTheirOriginalCadence()
        {
            var sigvaldFixture = new HeadlessGameFixture();
            var sigvald = sigvaldFixture.AddCard(
                sigvaldFixture.Game.Player1Index, "70038", RowPosition.MyCemetery);
            await sigvaldFixture.SynchronizeClientsAsync();

            Assert.Equal(2, sigvald.Status.Countdown);
            await sigvaldFixture.Game.SendEvent(
                new AfterTurnOver(sigvaldFixture.Game.Player2Index));
            Assert.Equal(2, sigvald.Status.Countdown);
            await sigvaldFixture.Game.SendEvent(
                new AfterTurnOver(sigvaldFixture.Game.Player1Index));
            Assert.Equal(1, sigvald.Status.Countdown);
            await sigvaldFixture.Game.SendEvent(
                new AfterTurnOver(sigvaldFixture.Game.Player1Index));
            Assert.True(sigvald.Status.CardRow.IsOnPlace());
            Assert.Equal(2, sigvald.Status.Countdown);
            Assert.Equal(2, sigvald.Status.Strength);

            var sorcererFixture = new HeadlessGameFixture();
            var sorcerer = sorcererFixture.AddCard(
                sorcererFixture.Game.Player1Index, CardId.WraithSorcerer, RowPosition.MyRow1);
            sorcerer.Status.IsLock = false;
            await sorcererFixture.SynchronizeClientsAsync();

            Assert.Equal(2, sorcerer.Status.Countdown);
            await sorcererFixture.Game.SendEvent(
                new AfterTurnStart(sorcererFixture.Game.Player2Index));
            Assert.Equal(2, sorcerer.Status.Countdown);
            await sorcererFixture.Game.SendEvent(
                new AfterTurnStart(sorcererFixture.Game.Player1Index));
            Assert.Equal(1, sorcerer.Status.Countdown);
            await sorcererFixture.Game.SendEvent(
                new AfterTurnStart(sorcererFixture.Game.Player1Index));
            Assert.True(sorcerer.Status.IsLock);
            Assert.Equal(2, sorcerer.Status.Countdown);

            var weakSorcererFixture = new HeadlessGameFixture();
            var weakSorcerer = weakSorcererFixture.AddCard(
                weakSorcererFixture.Game.Player1Index,
                CardId.WraithSorcerer,
                RowPosition.MyRow1,
                2);
            weakSorcerer.Status.IsLock = false;
            await weakSorcererFixture.SynchronizeClientsAsync();
            await weakSorcererFixture.Game.SendEvent(
                new AfterTurnStart(weakSorcererFixture.Game.Player1Index));
            await weakSorcererFixture.Game.SendEvent(
                new AfterTurnStart(weakSorcererFixture.Game.Player1Index));
            Assert.False(weakSorcerer.Status.IsLock);
            Assert.Equal(2, weakSorcerer.Status.Countdown);

            var queenFixture = new HeadlessGameFixture();
            var queen = queenFixture.AddCard(
                queenFixture.Game.Player1Index, CardId.EndregaQueen, RowPosition.MyRow1);
            await queenFixture.SynchronizeClientsAsync();

            Assert.Equal(3, queen.Status.Countdown);
            await queenFixture.Game.SendEvent(
                new AfterTurnOver(queenFixture.Game.Player1Index));
            Assert.Equal(2, queen.Status.Countdown);
            await queenFixture.Game.SendEvent(
                new AfterTurnOver(queenFixture.Game.Player1Index));
            Assert.Equal(1, queen.Status.Countdown);
            await queenFixture.Game.SendEvent(
                new AfterTurnOver(queenFixture.Game.Player1Index));
            Assert.Equal(3, queen.Status.Countdown);
            Assert.Contains(
                queenFixture.Game.PlayersPlace[queenFixture.Game.Player1Index][0],
                card => card.Status.CardId == CardId.EndregaEggs);
        }
    }
}
