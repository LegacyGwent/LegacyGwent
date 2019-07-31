using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24036")]//狂猎导航员
    public class WildHuntNavigator : CardEffect
    {//选择1个非“法师”的友军铜色“狂猎”单位，从牌组打出1张它的同名牌。
        public WildHuntNavigator(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = await Game.GetSelectPlaceCards(Card, 1, false, x => x.Is(Group.Copper, CardType.Unit, x => x.HasAllCategorie(Categorie.WildHunt) && !x.HasAllCategorie(Categorie.Mage) && Game.PlayersDeck[PlayerIndex].Any(t => t.Status.CardId == x.Status.CardId)), SelectModeType.MyRow);

            if (!targets.TrySingle(out var target))
            {
                return 0;
            }
            if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == target.Status.CardId).TryMessOne(out var card, Game.RNG))
            {
                return 0;
            }
            await card.MoveToCardStayFirst();
            return 1;
        }
    }
}