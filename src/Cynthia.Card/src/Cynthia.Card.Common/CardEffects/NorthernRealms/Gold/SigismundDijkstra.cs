using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42008")]//迪杰斯特拉
    public class SigismundDijkstra : CardEffect
    {//间谍。 从牌组随机打出2张牌。
        public SigismundDijkstra(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[AnotherPlayer].Mess(Game.RNG).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
            if (list.Count() == 1)
            {
                await list.First().MoveToCardStayFirst();
                return 1;
            }
            foreach (var card in list.Take(2).ToList())
            {
                await card.MoveToCardStayFirst();
            }
            return 2;
        }
    }
}