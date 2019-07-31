using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23006")]//卢恩
    public class Ruehin : CardEffect
    {//使位于手牌、牌组和己方半场自身外的所有“类虫生物”和“诅咒生物”单位获得1点强化。
        public Ruehin(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = Game.PlayersHandCard[PlayerIndex]
                .Concat(Game.PlayersDeck[PlayerIndex])
                .Concat(Game.GetPlaceCards(PlayerIndex))
                .FilterCards(type: CardType.Unit, filter: x => x.HasAnyCategorie(Categorie.Cursed, Categorie.Insectoid) && x != Card)
                .ToList();
            foreach (var target in targets)
            {
                await target.Effect.Strengthen(1, Card);
            }
            return 0;
        }
    }
}