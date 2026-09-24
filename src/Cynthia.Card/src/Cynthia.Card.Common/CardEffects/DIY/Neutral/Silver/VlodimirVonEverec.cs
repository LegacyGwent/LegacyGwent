using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.VlodimirVonEverec)]
    public class VlodimirVonEverec : CardEffect
    {
        public VlodimirVonEverec(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.IsAnyGroup(Group.Copper, Group.Silver) &&
                            x.Is(type: CardType.Unit) &&
                            x.HasAnyCategorie(Categorie.Witcher))
                .WhereAllLowest()
                .ToList();
            if (candidates.Count == 0)
            {
                return 0;
            }

            await candidates.Mess(RNG).First().MoveToCardStayFirst();
            return 1;
        }
    }
}
