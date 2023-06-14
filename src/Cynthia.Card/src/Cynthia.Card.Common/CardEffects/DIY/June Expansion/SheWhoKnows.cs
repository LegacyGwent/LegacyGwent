using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70085")]//南尼克
    public class SheWhoKnows : CardEffect
    {//将墓场3张铜色/银色单位牌放回牌组。
        public SheWhoKnows(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //参见issue#22，这里使用showcardmove
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.CardId == "24028"));
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 3, "拣选并放回最多三颗果子");
            foreach (var card in result)
            {
                card.Effect.Repair();
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), card);
            }
            await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()].SetStatus<ImpenetrableFogStatus>();
            return 0;
        }
    }
}