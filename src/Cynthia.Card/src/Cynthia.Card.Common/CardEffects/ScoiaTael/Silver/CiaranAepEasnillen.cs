using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53003")]//席朗·依斯尼兰
    public class CiaranAepEasnillen : CardEffect
    {//改变1个单位的锁定状态，并把它移至其所在半场的同排。
        public CiaranAepEasnillen(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            if (Game.PlayersPlace[AnotherPlayer].Count() >= Game.RowMaxCount)
            {
                return 0;
            }

            var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.AllRow, isHasConceal: true);

            foreach (var target in targets)
            {
                if (target.Status.CardRow == Card.Status.CardRow)
                {
                    await target.Effect.Lock(Card);
                    return 0;
                }
                else
                {
                    await target.Effect.Lock(Card);
                    await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
                    return 0;
                }
            }

            return 0;
        }
    }
}