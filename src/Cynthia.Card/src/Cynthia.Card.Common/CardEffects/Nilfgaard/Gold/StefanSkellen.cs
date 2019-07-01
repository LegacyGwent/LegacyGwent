using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32004")]//史提芬·史凯伦
    public class StefanSkellen : CardEffect
    {//将牌组任意1张卡牌移至顶端。若它为非间谍单位，则使其获得5点增益。
        public StefanSkellen(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var deck = Game.PlayersDeck[Card.PlayerIndex].ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, deck, 1);
            if (cards.Count() == 0) return 0;
            var card = cards.Single();
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), card);
            if (card.CardInfo().CardUseInfo == CardUseInfo.MyRow)
                await card.Effect.Boost(5, Card);
            return 0;
        }
    }
}