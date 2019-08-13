using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13023")]//黑血
    public class BlackBlood : CardEffect
    {//择一：创造1个铜色“食腐生物”或“吸血鬼”单位，并使其获得2点增益；或摧毁1个铜色/银色“食腐生物”或“吸血鬼”单位。
        public BlackBlood(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("挑拨", "创造1个铜色“食腐生物”或“吸血鬼”单位，并使其获得2点增益。"),
                ("剧毒", "摧毁1个铜色 / 银色“食腐生物”或“吸血鬼”单位。")
            );
            if (switchCard == 0)
            {
                var cards = GwentMap.GetCreateCardsId(x => x.Group == Group.Copper &&
                        (x.Categories.Contains(Categorie.Necrophage) ||
                        x.Categories.Contains(Categorie.Vampire)), Game.RNG).ToArray();
                if ((await Game.CreateAndMoveStay(PlayerIndex, cards, isCanOver: true)) == 1)
                {
                    await Game.PlayersStay[PlayerIndex].First().Effect.Boost(2, Card);
                    return 1;
                }
                return 0;
            }
            else if (switchCard == 1)
            {
                var target = await Game.GetSelectPlaceCards(Card, 1, false,
                x => x.HasAnyCategorie(Categorie.Necrophage, Categorie.Vampire) && x.IsAnyGroup(Group.Copper, Group.Silver));
                if (target.Count == 0) return 0;
                await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;
        }
    }
}