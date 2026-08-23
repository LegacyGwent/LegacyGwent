using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustTwentyThirdFirstBatchTests
    {
        [Fact]
        public async Task VesNeverSwapsAHandCardForTheSameCardId()
        {
            var fixture = new HeadlessGameFixture();
            ClearMutableZones(fixture);

            var ves = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Ves, RowPosition.MyRow1);
            var handWolf = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            var deckWolf = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var deckEskel = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            var swaps = new List<(string HandId, string DeckId)>();
            HeadlessGameFixture.ReplaceMainEffect(
                handWolf, new RecordingSwapEffect(handWolf, swaps));
            HeadlessGameFixture.ReplaceMainEffect(
                deckWolf, new RecordingSwapEffect(deckWolf, swaps));
            HeadlessGameFixture.ReplaceMainEffect(
                deckEskel, new RecordingSwapEffect(deckEskel, swaps));
            fixture.FirstPlayer.QueueMenuCardIds(CardId.Wolf, CardId.Eskel);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(
                () => ves.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(2, swaps.Count);
            Assert.All(swaps, swap => Assert.NotEqual(swap.HandId, swap.DeckId));
        }

        private static void ClearMutableZones(HeadlessGameFixture fixture)
        {
            foreach (var playerIndex in new[] { fixture.Game.Player1Index, fixture.Game.Player2Index })
            {
                fixture.Game.PlayersDeck[playerIndex].Clear();
                fixture.Game.PlayersHandCard[playerIndex].Clear();
                fixture.Game.PlayersCemetery[playerIndex].Clear();
                fixture.Game.PlayersStay[playerIndex].Clear();
                foreach (var row in fixture.Game.PlayersPlace[playerIndex])
                {
                    row.Clear();
                }
            }
        }

        private sealed class RecordingSwapEffect : CardEffect
        {
            private readonly List<(string HandId, string DeckId)> _swaps;

            public RecordingSwapEffect(
                GameCard card,
                List<(string HandId, string DeckId)> swaps) : base(card)
            {
                _swaps = swaps;
            }

            public override async Task Swap(GameCard target)
            {
                _swaps.Add((Card.Status.CardId, target.Status.CardId));
                await base.Swap(target);
            }
        }
    }
}
