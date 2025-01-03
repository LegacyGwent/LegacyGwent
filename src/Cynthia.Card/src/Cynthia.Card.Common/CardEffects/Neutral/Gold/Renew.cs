using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12035")]//复原
    public class Renew : CardEffect//, IHandlesEvent<AfterTurnStart>
    {//复活己方1个非领袖金色单位。
        public Renew(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.PlayersCemetery[PlayerIndex]
            .Where(x => x.Status.Group == Group.Gold && x.CardInfo().CardType == CardType.Unit && x.CardInfo().CardId != "70014").ToList();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list, 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count == 0) return 0;
            await result.Single().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
            // var result = await Game.GetSelectMenuCards(PlayerIndex, Game.GetAllCard(PlayerIndex), 100);
            // foreach (var card in result.Reverse())
            // {
            //     if (card.PlayerIndex == PlayerIndex)
            //         await card.MoveToCardStayFirst();
            //     else
            //         await card.MoveToCardStayFirst(true);
            // }
            // return result.Count;
            // return 0;
        }

        // public async Task HandleEvent(AfterTurnStart @event)
        // {
        //     foreach (var card in Game.PlayersCemetery[PlayerIndex].ToList())
        //     {
        //         await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), card);
        //     }
        //     if (!Card.Status.CardRow.IsInHand())
        //     {
        //         await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), Card);
        //     }
        // }
    }
}