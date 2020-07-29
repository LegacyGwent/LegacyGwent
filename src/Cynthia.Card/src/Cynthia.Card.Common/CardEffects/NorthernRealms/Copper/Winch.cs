using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44033")]//绞盘
    public class Winch : CardEffect
    {//使所有己方半场的“机械”单位获得3点增益。
        public Winch(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("复活", "从己方墓场中打出 1 张铜色“机械”牌，并使其获得佚亡。"),
                ("增益", "所有己方机械+3增益。")
            );

            if (switchCard == 0)
            {
                var list = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.Group == Group.Copper) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Machine)).ToList();
                if (list.Count() == 0)
                {
                    return 0;
                }
                //让玩家选择一张卡
                var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
                //如果玩家一张卡都没选择,没有效果
                if (result.Count() == 0)
                {
                    return 0;
                }
                var resurrectCard = result.First();
                resurrectCard.Status.IsDoomed = true;
                await resurrectCard.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
                return 1;
            }
            else if (switchCard == 1)
            {
                var list = Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Machine) && x.CardInfo().CardType == CardType.Unit);
                foreach (var card in list)
                {
                    await card.Effect.Boost(3, Card);
                }
                return 0;
            }

            return 0;
        }
    }
}