using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70137")]//巨橡 TheGreatOak
    public class TheGreatOak : CardEffect
    {//
        public TheGreatOak(GameCard card) : base(card) { }
        private GameCard DryadTarget = null;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("橡树之力", "选择一个友军单位，造成等同其基础战力的伤害。"),
                ("树精召唤", "重新打出1个银色/铜色友军树精单位。")
            );

            if (switchCard == 0)
            {
                var damageList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                if (!damageList.TrySingle(out var Dtarget))
                {
                    return 0;
                }
                await Dtarget.Effect.Weaken(Dtarget.Status.Strength/2, Card);
                return 0;
            }

            else if (switchCard == 1)
            {
                var list = Game.PlayersCemetery[Card.PlayerIndex]
                .Where(x => (x.Status.Group == Group.Copper) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Dryad));
                if (list.Count() == 0)
                {
                    return 0;
                }
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1);
                if (result.Count() == 0) return 0;
                DryadTarget = result.First();
                await DryadTarget.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
                return 1;
            }
            return 0;
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (DryadTarget == null)
            {
                return;
            }
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count)), DryadTarget);
            return;
        }

    }
}
