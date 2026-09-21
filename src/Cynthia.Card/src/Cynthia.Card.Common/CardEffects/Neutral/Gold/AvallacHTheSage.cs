using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12029")]//阿瓦拉克：贤者
    public class AvallacHTheSage : CardEffect
    {//检视对方牌组顶端3张不同品质的非间谍单位牌，生成其中1张的原始同名牌。
        public AvallacHTheSage(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var inspected = Game.PlayersDeck[AnotherPlayer]
                .Where(x => x.Status.Type == CardType.Unit &&
                            !x.Status.IsSpying &&
                            x.CardInfo().CardUseInfo != CardUseInfo.EnemyRow &&
                            x.CardInfo().CardUseInfo != CardUseInfo.EnemyPlace)
                .GroupBy(x => x.Status.Group)
                .Select(x => x.First())
                .Take(3)
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, inspected, isEnemyBack: false))
                .TrySingle(out var selected))
            {
                return 0;
            }
            await Game.CreateToStayFirst(selected.Status.CardId, PlayerIndex);
            return 1;
        }
    }
}
