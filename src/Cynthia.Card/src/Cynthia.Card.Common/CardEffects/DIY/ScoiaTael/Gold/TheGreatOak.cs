using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70137")]//巨橡 TheGreatOak
    public class TheGreatOak : CardEffect
    {//择一:削弱一个敌军单位一半的基础战力；打出1张铜色树精牌，随后将其放回牌组。
        public TheGreatOak(GameCard card) : base(card) { }
        private GameCard DryadTarget = null;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("橡树之力", "削弱一个敌军单位一半的基础战力"),
                ("树精召唤", "打出1张铜色树精牌，随后将其放回牌组")
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
                var list = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => (x.Status.Group == Group.Copper) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Dryad)).Mess(Game.RNG).ToList();
                if (list.Count() == 0)
                {
                    return 0;
                }
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
                if (result.Count() == 0) return 0;
                DryadTarget = result.First();
                await result.First().MoveToCardStayFirst();
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
