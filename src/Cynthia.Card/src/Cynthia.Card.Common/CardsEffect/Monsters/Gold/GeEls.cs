using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22013")]//盖尔
    public class GeEls : CardEffect
    {//随机检视牌组中1张金色牌和1张银色牌，打出1张，将另1张置于牌组顶端。
        public GeEls(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //先选出一张金，一张银
            var goldCandidate = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Gold).Mess().First();
            var silverCandidate = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Silver).Mess().First();

            var list=[goldCandidate, silverCandidate];

            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list, 1, "选择打出一张牌");

            //如果玩家一张卡都没选择, 两张都置于牌库顶
            if (result.Count() == 0)
            {
                //随机选择第一张，移动第一张
                Random rm = new Random();
                var cardToMove1=list.pop(rm.Next(list.Count));
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), cardToMove1);

                //移动第二张
                var cardToMove2=list[0];
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), cardToMove2);
                return 0;
            } 

        //如果玩家选了卡
        //先移动另一张卡到牌库顶
        cardToMove=list.Where(x => x != result.Single()).Single();
        await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), cardToMove);

        //打出所选的牌
        await result.Single().MoveToCardStayFirst();
        return 1;
        }
    }
}