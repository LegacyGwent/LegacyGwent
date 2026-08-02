using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("52013")]//伊森格林：亡命徒
    public class IsengrimOutlaw : CardEffect
    {//择一：从牌组打出1张铜色/银色“特殊”牌；或生成1个己方起始牌组之外的非间谍银色“精灵”单位。
        public IsengrimOutlaw(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var switchCard = await Card.GetMenuSwitch(
               ("暴行", "IsengrimOutlaw_1_PlaySpecial"),
               ("后援", "IsengrimOutlaw_2_CreateElf")
           );

            //从牌组打出1张铜色/银色“特殊”牌
            if (switchCard == 0)
            {
                //乱序列出铜色/银色“特殊”牌
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Special &&
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

                return await Card.CreateAndMoveStay(
                    GwentMap.GetGenerateCardsId(
                        x => x.Is(Group.Silver, CardType.Unit) &&
                            x.HasAnyCategorie(Categorie.Elf) &&
                            !x.HasAnyCategorie(Categorie.Agent),
                        Card.GetMyBaseDeck().Select(x => x.CardId))
                    .ToList());
            }

            return 0;
        }
    }
}
