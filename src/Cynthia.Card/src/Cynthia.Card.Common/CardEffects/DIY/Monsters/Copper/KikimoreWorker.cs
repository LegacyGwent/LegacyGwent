using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70022")]//齐齐摩工兵
    public class KikimoreWorker : CardEffect
    {
        //选择1个"类虫生物"单位，使其在手牌、牌组或己方半场所有同名牌获得2点增益。
        public KikimoreWorker(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var target = (await Game.GetSelectPlaceCards(Card, filter: (x => x.HasAnyCategorie(Categorie.Insectoid)), selectMode: SelectModeType.MyRow)).SingleOrDefault();
            if (target == default)
            {
                return 0;
            }
            var cards = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex]).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.Status.CardId == target.Status.CardId).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
    }
}

