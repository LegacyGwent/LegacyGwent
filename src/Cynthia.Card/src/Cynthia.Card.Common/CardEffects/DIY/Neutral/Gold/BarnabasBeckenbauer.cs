using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70125")]//巴纳巴斯·贝肯鲍尔 BarnabasBeckenbauer
    public class BarnabasBeckenbauer : CardEffect
    {//使1个其它友军单位获得2点增益，墓地中每有1种铜色道具牌便重复1次。
        public BarnabasBeckenbauer(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cardlist = Game.PlayersCemetery[PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.HasAllCategorie(Categorie.Item) && x.Status.Type == CardType.Special)
                .GroupBy(x => x.Status.CardId).ToList();
            var count = cardlist.Count();
            for (var i = 0; i < count; i++)
            {
                var cards = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
                if (cards.Count() == 0)
                {
                    return 0;
                }
                await cards.Single().Effect.Boost(2, Card);
            }
 

            return 0;
        }
    }
}
