using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("41004")]//亨赛特国王
    public class KingHenselt : CardEffect
    {//选择1个友军铜色“机械”或“科德温”单位，从牌组打出所有它的同名牌。 操控。
        public KingHenselt(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = await Game.GetSelectPlaceCards(Card, 1, false, x => x.Is(Group.Copper, CardType.Unit, x => x.HasAnyCategorie(Categorie.Machine, Categorie.Kaedwen) && Game.PlayersDeck[PlayerIndex].Any(t => t.Status.CardId == x.Status.CardId)), SelectModeType.MyRow);

            if (!targets.TrySingle(out var target))
            {
                return 0;
            }
            var list = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == target.Status.CardId).ToList();

            //两张打不行
            foreach (var card in list)
            {
                await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, card);
            }


            return list.Count();

        }
    }
}