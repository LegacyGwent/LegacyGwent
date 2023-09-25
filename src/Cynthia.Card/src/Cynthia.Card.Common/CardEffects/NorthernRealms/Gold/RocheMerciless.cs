using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("42007")]//罗契：冷酷之心
    public class RocheMerciless : CardEffect, IHandlesEvent<OnGameStart>
    {//“从手牌打出一张战力不高于自身的“泰莫利亚”单位，随后抽一张牌。对局开始时，将1个“蓝衣铁卫突击队”加入牌组”。
        public RocheMerciless(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersHandCard[Card.PlayerIndex]
                .Where(x => x.CardPoint() <= Card.CardPoint() && x.CardInfo().Categories.Contains((Categorie.Temeria)));
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, cards.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (!result.Any()) return 0;
            await result.Single().MoveToCardStayFirst();
            needdraw = true;
            // await Game.PlayerDrawCard(PlayerIndex, 1);

            return 1;
        }

        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if(needdraw)
            {
            await Game.PlayerDrawCard(Card.PlayerIndex, 1);
            }    
                return;
        }

        public async Task HandleEvent(OnGameStart @event)
        {
            await Game.CreateCard(CardId.BlueStripesCommando, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            return;
        }

        private bool needdraw = false;
    }
}
