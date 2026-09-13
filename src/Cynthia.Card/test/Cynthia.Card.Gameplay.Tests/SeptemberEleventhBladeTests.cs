using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberEleventhBladeTests
    {
        [Theory]
        [InlineData(Group.Copper, false, false)]
        [InlineData(Group.Silver, false, false)]
        [InlineData(Group.Gold, false, false)]
        [InlineData(Group.Gold, true, false)]
        [InlineData(Group.Copper, false, true)]
        public async Task BladeDealsTenDamageToSurvivorsWithoutChangingTheirDoomedStatus(
            Group group, bool cursed, bool doomed)
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f, f.Game.Player2Index, 20, group, cursed);
            target.Status.IsDoomed = doomed;
            await f.SynchronizeClientsAsync();

            await blade.Effect.CardUse();

            Assert.True(target.Status.CardRow.IsOnPlace());
            Assert.Equal(10, target.CardPoint());
            Assert.Equal(doomed, target.Status.IsDoomed);
            Assert.True(blade.Status.CardRow.IsInCemetery());
            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Theory]
        [InlineData(5, false, 15, 0)]
        [InlineData(20, false, 20, 10)]
        [InlineData(5, true, 20, 5)]
        public async Task BladeOrdinaryDamageRespectsArmorAndShield(
            int armor, bool shield, int remainingPower, int remainingArmor)
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f, f.Game.Player2Index, 20, Group.Copper, false);
            target.Status.Armor = armor;
            target.Status.IsShield = shield;
            await f.SynchronizeClientsAsync();

            await blade.Effect.CardUse();

            Assert.True(target.Status.CardRow.IsOnPlace());
            Assert.Equal(remainingPower, target.CardPoint());
            Assert.Equal(remainingArmor, target.Status.Armor);
            Assert.False(target.Status.IsShield);
            Assert.False(target.Status.IsDoomed);
        }

        [Theory]
        [InlineData(Group.Copper, 0, false, false)]
        [InlineData(Group.Silver, 0, false, false)]
        [InlineData(Group.Copper, 25, false, false)]
        [InlineData(Group.Silver, 5, true, false)]
        [InlineData(Group.Copper, 0, false, true)]
        public async Task BladeDamagesThenDestroysAndBanishesBronzeOrSilverCursedUnits(
            Group group, int armor, bool shield, bool allied)
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f,
                allied ? f.Game.Player1Index : f.Game.Player2Index, 30, group, true);
            target.Status.Armor = armor;
            target.Status.IsShield = shield;
            var observed = new RemovalObserver(target);
            HeadlessGameFixture.ReplaceMainEffect(target, observed);
            await f.SynchronizeClientsAsync();

            await blade.Effect.CardUse();

            Assert.Equal(RowPosition.Banish, target.Status.CardRow);
            Assert.Equal(new[] { "damage:10", "destroy:Scorch", "banish" }, observed.Steps);
            Assert.Equal(0, observed.Deaths);
            Assert.DoesNotContain(target, f.Game.PlayersCemetery[target.PlayerIndex]);
            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Theory]
        [InlineData(Group.Copper, false)]
        [InlineData(Group.Silver, false)]
        [InlineData(Group.Gold, true)]
        [InlineData(Group.Copper, true)]
        [InlineData(Group.Silver, true)]
        public async Task BladeBanishesDamageKillsWithoutDeathwishOrASecondDestroy(
            Group group, bool cursed)
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f, f.Game.Player2Index, 10, group, cursed);
            var observed = new RemovalObserver(target);
            HeadlessGameFixture.ReplaceMainEffect(target, observed);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await blade.Effects.RaiseEvent(new CardUseEffect()));

            Assert.Equal(RowPosition.Banish, target.Status.CardRow);
            Assert.Equal(new[] { "damage:10", "destroy:ToCemetery", "banish" }, observed.Steps);
            Assert.Equal(0, observed.Deaths);
            Assert.DoesNotContain(target, f.Game.PlayersCemetery[target.PlayerIndex]);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task BladeDamageKillAlsoBanishesWhenInvokedOutsideAnExistingTask()
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f, f.Game.Player2Index, 10, Group.Copper, false);
            await f.SynchronizeClientsAsync();

            await blade.Effect.CardUseEffect();

            Assert.Equal(RowPosition.Banish, target.Status.CardRow);
            Assert.DoesNotContain(target, f.Game.PlayersCemetery[target.PlayerIndex]);
        }

        [Fact]
        public async Task BladeReturnsFromCemeteryAndAddsThreeDamageOnEachReturn()
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f, f.Game.Player2Index, 100, Group.Gold, false);
            await f.SynchronizeClientsAsync();

            await blade.Effect.CardUse();
            Assert.Equal(90, target.CardPoint());
            Assert.True(blade.Status.CardRow.IsInCemetery());

            await EndRound(f, 1);
            Assert.Contains(blade, f.Game.PlayersDeck[f.Game.Player1Index]);
            Assert.DoesNotContain(blade, f.Game.PlayersCemetery[f.Game.Player1Index]);
            // Remaining in the deck at a later round end grants no extra damage.
            await EndRound(f, 2);
            await f.Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), blade);
            await blade.Effect.CardUse();
            Assert.Equal(77, target.CardPoint());

            await EndRound(f, 3);
            Assert.Contains(blade, f.Game.PlayersDeck[f.Game.Player1Index]);
            await f.Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), blade);
            await blade.Effect.CardUse();
            Assert.Equal(61, target.CardPoint());
            Assert.True(blade.Status.CardRow.IsInCemetery());
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task BladeGrowthBelongsToTheReturnedCopy()
        {
            var f = new HeadlessGameFixture();
            var returned = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyCemetery);
            var fresh = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            var target = AddTarget(f, f.Game.Player2Index, 100, Group.Gold, false);
            await f.SynchronizeClientsAsync();

            await EndRound(f, 1);
            await fresh.Effect.CardUse();
            Assert.Equal(90, target.CardPoint());
            await f.Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), returned);
            await returned.Effect.CardUse();
            Assert.Equal(77, target.CardPoint());
        }

        [Theory]
        [InlineData(RowPosition.MyHand)]
        [InlineData(RowPosition.MyDeck)]
        [InlineData(RowPosition.MyStay)]
        [InlineData(RowPosition.Banish)]
        public async Task BladeDoesNotReturnOrGrowOutsideTheCemetery(RowPosition row)
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade,
                row == RowPosition.Banish ? RowPosition.MyHand : row);
            var target = AddTarget(f, f.Game.Player2Index, 30, Group.Gold, false);
            await f.SynchronizeClientsAsync();
            if (row == RowPosition.Banish) await blade.Effect.Banish();

            await EndRound(f, 1);

            Assert.Equal(row, blade.Status.CardRow);
            if (row != RowPosition.Banish)
            {
                await f.Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), blade);
                await blade.Effect.CardUse();
                Assert.Equal(20, target.CardPoint());
            }
            else
            {
                Assert.DoesNotContain(blade, f.Game.PlayersDeck[f.Game.Player1Index]);
            }
        }

        [Fact]
        public async Task BladeCanResolveWithNoBoardTargetAndStillReturnNextRound()
        {
            var f = new HeadlessGameFixture();
            var blade = f.AddCard(f.Game.Player1Index, CardId.VandergriftSBlade, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await blade.Effect.CardUse();
            Assert.True(blade.Status.CardRow.IsInCemetery());
            await EndRound(f, 1);

            Assert.Contains(blade, f.Game.PlayersDeck[f.Game.Player1Index]);
            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        private static GameCard AddTarget(
            HeadlessGameFixture f, int playerIndex, int power, Group group, bool cursed)
        {
            var target = f.AddCard(playerIndex, CardId.GeraltOfRivia, RowPosition.MyRow1, power);
            target.Status.Group = group;
            target.Status.Categories = cursed ? new[] { Categorie.Cursed } : new[] { Categorie.Soldier };
            return target;
        }

        private static Task EndRound(HeadlessGameFixture f, int round)
        {
            return f.Game.AddTask(async () =>
                await f.Game.SendEvent(new AfterRoundOver(round, 0, 0, null)));
        }

        private sealed class RemovalObserver : CardEffect, IHandlesEvent<AfterCardDeath>
        {
            public RemovalObserver(GameCard card) : base(card) { }

            public List<string> Steps { get; } = new List<string>();
            public int Deaths { get; private set; }

            public override async Task Damage(int num, GameCard source,
                BulletType showType = BulletType.Arrow, bool isPenetrate = false,
                DamageType damageType = DamageType.Unit)
            {
                Steps.Add($"damage:{num}");
                await base.Damage(num, source, showType, isPenetrate, damageType);
            }

            public override Task ToCemetery(CardBreakEffectType type = CardBreakEffectType.ToCemetery,
                bool isNeedBanish = true, bool isNeedSentEvent = true)
            {
                Steps.Add($"destroy:{type}");
                return base.ToCemetery(type, isNeedBanish, isNeedSentEvent);
            }

            public override Task Banish()
            {
                Steps.Add("banish");
                return base.Banish();
            }

            public Task HandleEvent(AfterCardDeath @event)
            {
                if (@event.Target == Card) Deaths++;
                return Task.CompletedTask;
            }
        }
    }
}
