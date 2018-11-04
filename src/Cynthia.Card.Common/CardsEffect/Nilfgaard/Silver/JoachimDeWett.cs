using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("63001")]//约阿希姆·德·维特
    public class JoachimDeWett : CardEffect
    {
        public JoachimDeWett(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)]
            .Where(x => (x.Status.Group == Group.Copper || Card.Status.Group == Group.Silver) &&//铜色或者银色
                    x.CardInfo().CardUseInfo == CardUseInfo.MyRow &&//忠诚
                    x.CardInfo().CardType == CardType.Unit);//单位牌
            if (list.Count() == 0) return 0;
            var moveCard = list.First();
            await moveCard.MoveToCardStayFirst();
            await moveCard.Effect.Boost(10);
            return 1;
        }
    }
}