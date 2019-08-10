using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23019")]//维尔金的女巨魔
    public class SheTrollOfVergen : CardEffect
    {//从牌组打出1个铜色“遗愿”单位。吞噬它并获得其基础战力的增益。
        public SheTrollOfVergen(GameCard card) : base(card) { }
        private GameCard _target = null;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            _target = null;
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.HideTags.Contains(HideTag.Deathwish) && x.Is(Group.Copper)).ToList();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list, 1, isCanOver: false);
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.Single().MoveToCardStayFirst();
            _target = result.Single();
            return 1;
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (_target == null)
            {
                return;
            }
            await Consume(_target, x => x.Status.Strength);
        }
    }
}