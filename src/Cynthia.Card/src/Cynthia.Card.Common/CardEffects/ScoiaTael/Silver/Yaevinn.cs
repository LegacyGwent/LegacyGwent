using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("53001")]//亚伊文
    public class Yaevinn : CardEffect
    {//间谍、力竭。 抽1张“特殊”牌和单位牌。保留1张，放回另一张。
        public Yaevinn(GameCard card) : base(card) { }
        public bool IsUse = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (IsUse)
            {
                return 0;
            }
            IsUse = true;
            //先选出一张单位卡，一张特殊卡
            var specialCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.HasAllCategorie(Categorie.Special));

            var unitCandidate = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG)
                .FirstOrDefault(x => x.CardInfo().CardType == CardType.Unit);

            var list = new List<GameCard>();

            if (specialCandidate != default)
            {
                list.Add(specialCandidate);
            }

            if (unitCandidate != default)
            {
                list.Add(unitCandidate);
            }
            //让玩家选择一张牌
            if (list.Count() == 0)
            {
                return 0;
            }
            //对于抽取的卡
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1, "选择抽一张牌", isCanOver: false);
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(dcard.PlayerIndex);//抽卡

            //对于另一张卡，放回意味着这样卡到了另一个卡组随机位置
            if (list.Count() == 2)
            {
                foreach (var card in list)
                {
                    if (card != dcard)
                    {
                        //放回到随机位置
                        await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), card);
                    }
                }

            }

            return 0;
        }
    }
}