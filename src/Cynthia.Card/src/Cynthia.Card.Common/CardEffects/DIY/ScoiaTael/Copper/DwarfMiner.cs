using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70097")]//矮人矿工
    public class DwarfMiner : CardEffect
    {//手卡中每有一张矮人单位卡便获得1点强化
        public DwarfMiner (GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var handDwarf = Game.PlayersHandCard[PlayerIndex].FilterCards(filter: x => x.HasAnyCategorie(Categorie.Dwarf)).ToList();
            var count = handDwarf.Count();
            await Card.Effect.Strengthen(count, Card);
            return 0;
        }
    }
}