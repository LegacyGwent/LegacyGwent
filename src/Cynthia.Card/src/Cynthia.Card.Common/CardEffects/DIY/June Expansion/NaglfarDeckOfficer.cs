using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70092")] 
    public class NaglfarDeckOfficer : CardEffect
    {
        //Resurect and banish a bronze Hazard
        public NaglfarDeckOfficer(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersCemetery[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper &&
                            (x.CardInfo().Categories.Contains(Categorie.Hazard))).Mess(RNG);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (!result.Any()) return 0;
            var card = result.Single();
            card.Status.IsDoomed = true;
            await card.Effect
                .Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
        }
    }
}