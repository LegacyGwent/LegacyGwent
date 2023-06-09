using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33021")]//吊死鬼之毒
    public class Cadaverine : CardEffect
    {//择一：对1个敌军单位以及所有与它同类型的单位造成3点伤害；或摧毁1个铜色/银色“中立”单位。
        public Cadaverine(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var switchCard = await Card.GetMenuSwitch
            (
                ("毒药", "对1个敌军单位以及所有与它同类型的单位造成3点伤害。"),
                ("强酸", "摧毁1个铜色/银色“中立”单位。")
            );
            if (switchCard == 0)
            {
                var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                if (cards.Count == 0)
                {
                    return 0;
                }
                var categories = cards.Single().Status.Categories;
                await Game.Debug("标签开始筛选:" + categories.Join(","));
                var targetCards = Game.GetAllCard(PlayerIndex).Where(x => x.PlayerIndex == AnotherPlayer && x.Status.CardRow.IsOnPlace() && x.Status.Categories.Intersect(categories).Any());
                await Game.Debug($"筛选出了{targetCards.Count()}个");
                foreach (var card in targetCards)
                {
                    await card.Effect.Damage(3, Card, BulletType.RedLight);
                }
            }
            else if (switchCard == 1)
            {
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => ((x.Status.Group == Group.Copper) &&//铜色
                    (x.CardInfo().CardType == CardType.Unit))).Mess(RNG).ToList();//单位牌
            if (list.Count() == 0) return 0;
            var moveCard = list.First();
            await moveCard.MoveToCardStayFirst();
            return 1;
            }

            return 0;
        }
    }
}