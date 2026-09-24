using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.SkjordalDrummond)]
    public class SkjordalDrummond : CardEffect
    {
        private static readonly Categorie[] Clans =
        {
            Categorie.ClanDrummond, Categorie.ClanTuirseach, Categorie.ClanDimun,
            Categorie.ClanTordarroch, Categorie.ClanHeymaey, Categorie.ClanAnCraite,
            Categorie.ClanBrokvar
        };

        public SkjordalDrummond(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var discardCandidates = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.Is(Group.Copper, CardType.Unit) &&
                            Clans.Any(clan => x.HasAnyCategorie(clan)))
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, discardCandidates, 1,
                    "选择丢弃一张牌")).TrySingle(out var discarded))
            {
                return 0;
            }

            var clan = Clans.First(x => discarded.HasAnyCategorie(x));
            await discarded.Effect.Discard(Card);
            var resurrectCandidates = Game.PlayersCemetery[PlayerIndex]
                .Where(x => x.Is(Group.Copper, CardType.Unit) &&
                            x.Status.CardId != discarded.Status.CardId &&
                            x.HasAnyCategorie(clan))
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, resurrectCandidates, 1,
                    "选择复活一张牌")).TrySingle(out var resurrected))
            {
                return 0;
            }

            await resurrected.Effect.Resurrect(
                new CardLocation(RowPosition.MyStay, 0), Card);
            return 1;
        }
    }
}
