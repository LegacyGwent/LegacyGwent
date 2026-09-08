using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberSecondFirstBatchTests
    {
        [Fact]
        public async Task IhuarraquaxLetsItsOwnerChooseThreeEnemyTargets()
        {
            var fixture = new HeadlessGameFixture();
            var ihuarraquax = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Ihuarraquax, RowPosition.MyRow1);
            var enemies = Enumerable.Range(0, 4)
                .Select(_ => fixture.AddCard(
                    fixture.Game.Player2Index,
                    CardId.GeraltOfRivia,
                    RowPosition.MyRow1,
                    strength: 10))
                .ToArray();
            ihuarraquax.Status.HealthStatus = 0;
            ihuarraquax.Status.IsCountdown = true;
            ihuarraquax.Status.Countdown = 1;
            await fixture.SynchronizeClientsAsync();

            await ihuarraquax.Effects.RaiseEvent(
                new AfterTurnOver(fixture.Game.Player1Index));

            Assert.All(enemies.Take(3), enemy => Assert.Equal(-7, enemy.Status.HealthStatus));
            Assert.Equal(0, enemies[3].Status.HealthStatus);
        }

        [Fact]
        public async Task NorthernDraugOffersAtMostEightCemeteryTargetsAndResurrectsTheSelection()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersCemetery[fixture.Game.Player1Index].Clear();
            var draug = fixture.AddCard(
                fixture.Game.Player1Index, CardId.NorthernRealmsDraug, RowPosition.MyRow1);
            var corpses = Enumerable.Range(0, 10)
                .Select(_ => fixture.AddCard(
                    fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery))
                .ToArray();
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await draug.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(8, corpses.Count(card => card.Status.CardRow.IsOnPlace()));
            Assert.Equal(2, fixture.Game.PlayersCemetery[fixture.Game.Player1Index].Count);
            Assert.All(
                corpses.Where(card => card.Status.CardRow.IsOnPlace()),
                card =>
                {
                    Assert.Equal(CardId.Draugir, card.Status.CardId);
                    Assert.Equal(1, card.Status.Strength);
                });
        }

        [Fact]
        public async Task ReinforcedTrebuchetStrengthensOnceForEachCrewTrigger()
        {
            var fixture = new HeadlessGameFixture();
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.SiegeSupport, RowPosition.MyRow1);
            var trebuchet = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ReinforcedTrebuchet, RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.SiegeSupport, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await trebuchet.Effect.CardPlayEffect(false, false);

            Assert.Equal(10, trebuchet.Status.Strength);
        }

        [Fact]
        public async Task MorennBanishesLethalTargetsWithoutDoomingSurvivors()
        {
            var lethalFixture = new HeadlessGameFixture();
            var lethalMorenn = lethalFixture.AddCard(
                lethalFixture.Game.Player1Index, CardId.Morenn, RowPosition.MyRow1);
            var lethalTarget = lethalFixture.AddCard(
                lethalFixture.Game.Player2Index, CardId.HarpyEgg, RowPosition.MyRow1);
            lethalMorenn.Status.Conceal = true;
            await lethalFixture.SynchronizeClientsAsync();

            await lethalFixture.Game.AddTask(async () =>
                await lethalFixture.Game.SendEvent(new AfterUnitDown(
                    lethalTarget, true, false, (false, false), false, true)));

            Assert.Equal(RowPosition.Banish, lethalTarget.Status.CardRow);
            Assert.DoesNotContain(
                lethalFixture.Game.GetPlaceCards(lethalFixture.Game.Player2Index),
                card => card.Status.CardId == CardId.HarpyHatchling);

            var survivingFixture = new HeadlessGameFixture();
            var survivingMorenn = survivingFixture.AddCard(
                survivingFixture.Game.Player1Index, CardId.Morenn, RowPosition.MyRow1);
            var survivor = survivingFixture.AddCard(
                survivingFixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 10);
            survivingMorenn.Status.Conceal = true;
            await survivingFixture.SynchronizeClientsAsync();

            await survivingFixture.Game.AddTask(async () =>
                await survivingFixture.Game.SendEvent(new AfterUnitDown(
                    survivor, true, false, (false, false), false, true)));

            Assert.True(survivor.Status.CardRow.IsOnPlace());
            Assert.Equal(-7, survivor.Status.HealthStatus);
            Assert.False(survivor.Status.IsDoomed);
        }

        [Fact]
        public async Task MorennForestChildCancelsGoldSpecialCards()
        {
            var fixture = new HeadlessGameFixture();
            var morenn = fixture.AddCard(
                fixture.Game.Player1Index, CardId.MorennForestChild, RowPosition.MyRow1);
            var special = fixture.AddCard(
                fixture.Game.Player2Index, CardId.RoyalDecree, RowPosition.MyHand);
            morenn.Status.Conceal = true;
            var beforePlay = new BeforeSpecialPlay(special);
            await fixture.SynchronizeClientsAsync();

            await morenn.Effects.RaiseEvent(beforePlay);

            Assert.False(beforePlay.IsUse);
            Assert.False(morenn.Status.Conceal);
        }

        [Fact]
        public async Task CongregationCopyOfBoostedWraithSorcererLocksAtThreeTotalPower()
        {
            var fixture = new HeadlessGameFixture();
            var cleric = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CongregationCleric, RowPosition.MyRow1);
            var original = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.WraithSorcerer,
                RowPosition.MyRow2,
                strength: 3);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () => await original.Effect.Lock(original));
            var generated = fixture.Game.GetPlaceCards(fixture.Game.Player1Index)
                .Single(card => card.Status.CardId == CardId.WraithSorcerer && card != original);
            Assert.Equal(2, generated.Status.Strength);
            await generated.Effect.Boost(1, cleric);

            await generated.Effects.RaiseEvent(
                new AfterTurnStart(fixture.Game.Player1Index));
            await generated.Effects.RaiseEvent(
                new AfterTurnStart(fixture.Game.Player1Index));

            Assert.Equal(3, generated.CardPoint());
            Assert.True(generated.Status.IsLock);
        }
    }
}
