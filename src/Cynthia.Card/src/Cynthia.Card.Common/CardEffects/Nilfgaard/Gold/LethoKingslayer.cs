using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32011")]//雷索：弑王者
    public class LethoKingslayer : CardEffect
    {
        //择一：摧毁1名敌军领袖，自身获得5点增益；或从牌组打出1张铜色/银色“谋略”牌。
        public LethoKingslayer(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //选择界面
            var switchCard = await Card.GetMenuSwitch
            (
                ("屠宰", "摧毁1名敌军领袖，自身获得5点增益。"),
                ("诡计", "从牌组打出1张铜色/银色“谋略”牌。")
            );
            //选项1
            if (switchCard == 0)
            {
                //1的效果
                var target = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.Group == Group.Leader);
                if (target.Count == 0) return 0;
                await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
                await Boost(5, Card);
            }
            //选项2
            else if (switchCard == 1)
            {
                //2的效果
                var cards = Game.PlayersDeck[Card.PlayerIndex]
                    .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.Status.Categories.Contains(Categorie.Tactic))
                    .ToList();
                //让玩家选择一张卡
                var result = await Game.GetSelectMenuCards(PlayerIndex, cards);
                if (result.Count() == 0) return 0;
                await result.First().MoveToCardStayFirst();
                return 1;
            }
            return 0;
        }
    }
}