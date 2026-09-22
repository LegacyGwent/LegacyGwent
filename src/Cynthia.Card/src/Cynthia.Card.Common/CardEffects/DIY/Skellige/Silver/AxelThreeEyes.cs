using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70203")]
    public class AxelThreeEyes : Choose
    {
        public AxelThreeEyes(GameCard card) : base(card) { }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    foreach (var row in TurnType.My.GetRow())
                    {
                        if (Game.RowToList(PlayerIndex, row).Count < Game.RowMaxCount)
                        {
                            await Game.CreateCardAtEnd(CardId.Crow, PlayerIndex, row, source: Card);
                        }
                    }
                    break;
                case 2:
                    for (var count = 0; count < 2; count++)
                    {
                        await Game.CreateCard(
                            CardId.CrowSEye,
                            PlayerIndex,
                            new CardLocation(RowPosition.MyDeck, Game.PlayersDeck[PlayerIndex].Count),
                            source: Card);
                    }
                    var topCrowEye = Game.PlayersDeck[PlayerIndex]
                        .First(card => card.Status.CardId == CardId.CrowSEye);
                    await topCrowEye.MoveToCardStayFirst();
                    return 1;
            }

            return 0;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>
            {
                { 1, "AxelThreeEyes_1_SpawnCrows" },
                { 2, "AxelThreeEyes_2_CreateCrowEyes" }
            };
        }
    }
}
