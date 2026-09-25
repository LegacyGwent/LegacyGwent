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
                .ToList();
            if (candidates.Count == 0)
            {
                return 0;
            }

            var selected = await Game.GetSelectMenuCards(PlayerIndex, candidates, 1, isCanOver: false);
            if (!selected.TrySingle(out var witcher))
            {
                return 0;
            }

            await witcher.MoveToCardStayFirst();
            return 1;
        }
    }
}
