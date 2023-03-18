using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70112")]//原始野性
    public class PrimitiveWildness : CardEffect
    {//对一个友军单位造成3点伤害，然后随机从牌库打出一个铜色呓语单位。
        public PrimitiveWildness(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit) && x.HasAllCategorie(Categorie.Cultist))).Mess(Game.RNG).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
            var moveCard = list.First();
            
            
            var targets = await Game.GetSelectPlaceCards(Card, 1,  selectMode: SelectModeType.MyRow);

            if (!targets.TrySingle(out var target))
            {
                return 0;
            }

            
            await target.Effect.Damage(3, Card);

            await moveCard.MoveToCardStayFirst();
            return 1;
            


        }
    }
}