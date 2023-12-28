using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70098")] //维里赫德旅破坏者 VriheddSaboteu
    public class VriheddSaboteur : CardEffect
    {//随机打出1张铜色道具牌，若牌组数量低于自身战力，改为复活1张铜色道具牌。
        public VriheddSaboteur(GameCard card) : base(card){}

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int DeckCount = Game.PlayersDeck[Card.PlayerIndex].Count();
            if (DeckCount >= Card.CardPoint())
            {
                var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Item) && x.CardInfo().CardType == CardType.Special)).ToList();
                if (list.Count() == 0) return 0;
                var moveCard = list.Mess(RNG).First();
                await moveCard.MoveToCardStayFirst();
                return 1;
            }
            else
            {
                var list = Game.PlayersCemetery[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper &&(x.CardInfo().Categories.Contains(Categorie.Item))).Mess(RNG);
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
                if (!result.Any()) return 0;
                var moveCard = result.Single();
                await moveCard.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
                return 1;
            }
         }
    }
}
