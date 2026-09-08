using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public sealed class SeptemberSeventhFirstBatchTests
    {
        [Fact]
        public async Task OriginalDraugUsesRandomResurrectionWithoutAMenuOrEightCardLimit()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.RowMaxCount = 12;
            var draug = fixture.AddCard(0, CardId.Draug, RowPosition.MyRow1);
            for (var i = 0; i < 14; i++)
                fixture.AddCard(0, CardId.Wolf, RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();
            await fixture.Game.AddTask(async () => await draug.Effects.RaiseEvent(new CardPlayEffect(false, false)));
            Assert.Empty(fixture.FirstPlayer.MenuRequests);
            Assert.Equal(12, fixture.Game.PlayersPlace[0][0].Count);
            Assert.Equal(3, fixture.Game.PlayersCemetery[0].Count);
            Assert.All(fixture.Game.PlayersPlace[0][0].Where(card => card != draug), card =>
            {
                Assert.Equal(CardId.Draugir, card.Status.CardId);
                Assert.Equal(1, card.Status.Strength);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task NorthernDraugAllowsPartialOrEmptySelectionAndExcludesSpecials(int count)
        {
            var fixture = new HeadlessGameFixture();
            var draug = fixture.AddCard(0, CardId.NorthernRealmsDraug, RowPosition.MyRow2);
            var special = fixture.AddCard(0, CardId.FirstLight, RowPosition.MyCemetery);
            var corpses = Enumerable.Range(0, 3)
                .Select(_ => fixture.AddCard(0, CardId.Wolf, RowPosition.MyCemetery)).ToArray();
            fixture.FirstPlayer.MenuSelectionOverride = menu => Enumerable.Range(0, count).ToList();
            await fixture.SynchronizeClientsAsync();
            await fixture.Game.AddTask(async () => await draug.Effects.RaiseEvent(new CardPlayEffect(false, false)));
            var menu = Assert.Single(fixture.FirstPlayer.MenuRequests);
            Assert.True(menu.IsCanOver);
            Assert.Equal(3, menu.SelectCount);
            Assert.DoesNotContain(menu.SelectList, card => card.CardId == special.Status.CardId);
            Assert.Equal(count, corpses.Count(card => card.Status.CardRow == RowPosition.MyRow2));
            Assert.Contains(special, fixture.Game.PlayersCemetery[0]);
            Assert.All(corpses.Take(count), card => Assert.Equal(1, card.Status.Strength));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task NorthernDraugDoesNotPromptWhenRowIsFullOrNoUnitIsInCemetery(bool fullRow)
        {
            var fixture = new HeadlessGameFixture();
            var draug = fixture.AddCard(0, CardId.NorthernRealmsDraug, RowPosition.MyRow1);
            if (fullRow)
            {
                for (var i = 1; i < fixture.Game.RowMaxCount; i++)
                    fixture.AddCard(0, CardId.Wolf, RowPosition.MyRow1);
                fixture.AddCard(0, CardId.Wolf, RowPosition.MyCemetery);
            }
            else fixture.AddCard(0, CardId.FirstLight, RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();
            await fixture.Game.AddTask(async () => await draug.Effects.RaiseEvent(new CardPlayEffect(false, false)));
            Assert.Empty(fixture.FirstPlayer.MenuRequests);
            Assert.Single(fixture.Game.PlayersCemetery[0]);
        }

        [Fact]
        public async Task CalantheExcludesIntrinsicAndStatusSpiesButPlaysTheChosenNormalUnit()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[0].Clear();
            var intrinsicSpy = fixture.AddCard(0, "33002", RowPosition.MyDeck); // False Ciri.
            Assert.False(intrinsicSpy.Status.IsSpying);
            Assert.Equal(CardUseInfo.EnemyRow, intrinsicSpy.CardInfo().CardUseInfo);
            var statusSpy = fixture.AddCard(0, CardId.Wolf, RowPosition.MyDeck);
            statusSpy.Status.IsSpying = true;
            var normal = fixture.AddCard(0, CardId.ArachasHatchling, RowPosition.MyDeck);
            var special = fixture.AddCard(0, CardId.FirstLight, RowPosition.MyDeck);
            var gold = fixture.AddCard(0, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var returned = fixture.AddCard(0, CardId.AnCraiteGreatsword, RowPosition.MyRow1);
            var calanthe = fixture.AddCard(0, CardId.QueenCalanthe, RowPosition.MyHand);
            fixture.FirstPlayer.QueueMenuCardIds(normal.Status.CardId);
            await fixture.SynchronizeClientsAsync();
            await calanthe.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));
            var menu = Assert.Single(fixture.FirstPlayer.MenuRequests);
            Assert.Equal(2, menu.SelectList.Count);
            Assert.Contains(menu.SelectList, card => card.CardId == normal.Status.CardId);
            Assert.Contains(menu.SelectList, card => card.CardId == returned.Status.CardId);
            foreach (var excluded in new[] { intrinsicSpy, statusSpy, special, gold })
            {
                Assert.DoesNotContain(menu.SelectList, card => card.CardId == excluded.Status.CardId);
                Assert.Contains(excluded, fixture.Game.PlayersDeck[0]);
            }
            Assert.True(normal.Status.CardRow.IsOnPlace());
            Assert.Equal(0, normal.PlayerIndex);
        }

        [Theory]
        [InlineData(-1, 0, true)]
        [InlineData(0, 2, true)]
        [InlineData(-1, 2, true)]
        [InlineData(0, 0, false)]
        [InlineData(2, 0, false)]
        public async Task MadChargeAcceptsWoundedOrArmoredAllies(int health, int armor, bool accepted)
        {
            var fixture = new HeadlessGameFixture();
            var charge = fixture.AddCard(0, "70050", RowPosition.MyStay);
            var ally = fixture.AddCard(0, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            ally.Status.HealthStatus = health;
            ally.Status.Armor = armor;
            var enemy = fixture.AddCard(1, CardId.Eskel, RowPosition.MyRow1, strength: 1);
            await fixture.SynchronizeClientsAsync();
            await fixture.Game.AddTask(async () => await charge.Effect.CardUseEffect());
            Assert.Equal(accepted, fixture.Game.PlayersCemetery[1].Contains(enemy));
            Assert.Equal(accepted ? 2 : 0, fixture.FirstPlayer.PlaceSelectionSources.Count);
        }
    }
}
