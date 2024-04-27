using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70022")]//齐齐摩工兵
    public class KikimoreWorker : CardEffect
    {//选择1个"类虫生物"单位，使其在手牌、牌组或己方半场所有同名牌获得2点增益。
        public KikimoreWorker(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var boostlist = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && x.HasAllCategorie(Categorie.Insectoid) && x != Card).ToList();;
            foreach (var card in boostlist)
            {
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
    }
}

