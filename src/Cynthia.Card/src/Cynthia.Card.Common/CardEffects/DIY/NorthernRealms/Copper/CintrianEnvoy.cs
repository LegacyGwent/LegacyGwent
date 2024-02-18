using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70144")]//辛特拉使者
    public class CintrianEnvoy : CardEffect
    {//择一：打出1张自身的同名牌。
        public CintrianEnvoy(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            var ids = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.Cintra))
                    .Select(x => x.CardId);
            if (!ids.TryMessOne(out var createId, Game.RNG))
            {
                return 0;
            }
            if (list.Count() == 0)
            {
                await Game.CreateCardAtEnd(createId, PlayerIndex, RowPosition.MyDeck);
                return 0;
            }
            await list.First().MoveToCardStayFirst();
            return 1;
        }
    }
}

