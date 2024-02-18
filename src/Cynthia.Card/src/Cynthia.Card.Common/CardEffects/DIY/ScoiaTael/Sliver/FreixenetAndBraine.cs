using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70105")] // 菲斯奈特与布雷恩 FreixenetAndBraine
    public class FreixenetAndBraine : CardEffect
    {//随机打出1张铜色士兵牌，若牌组数量低于自身战力，则改为打出1张铜色牌。
        public FreixenetAndBraine(GameCard card) : base(card){}

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int DeckCount = Game.PlayersDeck[Card.PlayerIndex].Count();
            if (DeckCount >= Card.CardPoint())
            {
                var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Soldier) && x.CardInfo().CardType == CardType.Unit)).ToList();
                if (list.Count() == 0) return 0;
                var moveCard = list.Mess(RNG).First();
                await moveCard.MoveToCardStayFirst();
                return 1;
            }
            else
            {
                var list = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver)).Mess(RNG);
                var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
                if (result.Count() == 0) return 0;
                await result.First().MoveToCardStayFirst();
                return 1;
            }
         }
    }
}

