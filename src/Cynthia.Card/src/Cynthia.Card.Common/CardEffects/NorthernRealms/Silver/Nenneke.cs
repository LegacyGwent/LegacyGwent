using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43006")]//南尼克
    public class Nenneke : CardEffect
    {//将墓场3张铜色/银色单位牌放回牌组。
        public Nenneke(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //参见issue#22，这里使用showcardmove
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 3, "选则放回三张牌");
            foreach (var card in result)
            {
                card.Effect.Repair();
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), card);
            }

            return 0;
        }
    }
}