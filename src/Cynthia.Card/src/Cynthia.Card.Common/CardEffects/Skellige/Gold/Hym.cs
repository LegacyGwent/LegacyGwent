using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62012")] //希姆
    public class Hym : CardEffect
    { //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public Hym(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择选项,设置每个选项的名字和效果
            //选项名来源 https://vmobile.douyu.com/show/a4Jj7lZXpoqWDk01?share_source=2 5分钟往后一点
            var switchCard = await Card.GetMenuSwitch(
                ("寄生之缚", "从牌组打出1张铜色/银色“诅咒生物”牌"),
                ("低语", "创造对方初始牌组中1张银色单位牌")
            );

            //选择了打诅咒生物的话
            if (switchCard == 0)
            {
                //乱序列出诅咒生物，如果没有，什么都不做
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Cursed) &&
                       (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                    .Mess(Game.RNG)
                    .ToList();

                if (list.Count() == 0)
                {
                    return 0;
                }
                //选一张，如果没选，什么都不做
                var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
                if (cards.Count() == 0)
                {
                    return 0;
                }

                //打出
                var playCard = cards.Single();
                await playCard.MoveToCardStayFirst();
                return 1;
            }

            //选择创造
            else if (switchCard == 1)
            {
                //手动排除大间谍
                var cardsId = Game.PlayerBaseDeck[AnotherPlayer].Deck
                   .Distinct()
                   .Where(x => x.HasAnyCategorie(Categorie.Agent))
                   .Mess(Game.RNG)
                   .Take(3)
                   .Select(x => x.CardId).ToArray();
                return await Game.CreateAndMoveStay(PlayerIndex, cardsId);

            }

            return 0;
        }
    }
}