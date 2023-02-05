using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{

    [CardEffectId("53001")] //亚伊文
    public class Yaevinn : CardEffect
    {
        //间谍、力竭。 抽1张“特殊”牌和单位牌。保留1张，放回另一张。
        private bool isUse = false;

        public Yaevinn(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (isUse)
            {
                return 0;
            }

            isUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            var card1 = Game.PlayersDeck[AnotherPlayer].FilterCards(type: CardType.Special).Mess(Game.RNG)
                .FirstOrDefault();
            var card2 = Game.PlayersDeck[AnotherPlayer].FilterCards(type: CardType.Unit).Mess(Game.RNG)
                .FirstOrDefault();

            var list = new List<GameCard>();
            if (card1 != null) list.Add(card1);
            if (card2 != null) list.Add(card2);

            //让玩家(另一半场)选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(Game.AnotherPlayer(Card.PlayerIndex), list, isCanOver: false);
            if (result.Count == 0) return 0;
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0); //将选中的卡移动到最上方
            await Game.PlayerDrawCard(Game.AnotherPlayer(Card.PlayerIndex)); //抽卡

            return 0;
        }
    }
}