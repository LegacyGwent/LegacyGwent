using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34003")]//近卫军
    public class ImperaBrigade : CardEffect
    {
        public ImperaBrigade(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //对方半场的间谍数量
            var count = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.PlayerIndex == Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.Status.CardRow.IsOnPlace() && x.Status.IsSpying).Count();
            //每有一个间谍获得2点增益
            await Boost(count * 2);
            return 0;
        }
        public override async Task OnCardSpyingChange(GameCard taget, bool isSpying, GameCard soure = null)
        {
            //如果在场上, 并且对方半场出现间谍, 增益2点
            if (Card.Status.CardRow.IsOnPlace() && taget.PlayerIndex != Card.PlayerIndex && isSpying)
                await Boost(2);
        }
    }
}