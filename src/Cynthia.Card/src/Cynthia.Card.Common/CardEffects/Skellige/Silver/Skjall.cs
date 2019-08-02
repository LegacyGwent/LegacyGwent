using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63015")]//史凯裘
    public class Skjall : CardEffect
    {//从牌组随机打出1张铜色/银色“诅咒生物”单位牌。
        public Skjall(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {	

            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => ((x.Status.Group == Group.Copper||x.Status.Group == Group.Silver) &&(x.CardInfo().CardType == CardType.Unit)&&x.HasAllCategorie(Categorie.Cursed))).Mess().ToList();
            if (list.Count() == 0) return 0;
            var moveCard = list.First();
            await moveCard.MoveToCardStayFirst();
            return 1;
        }
    }
}