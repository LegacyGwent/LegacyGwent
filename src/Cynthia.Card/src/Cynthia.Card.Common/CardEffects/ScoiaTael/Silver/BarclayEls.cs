using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53017")] //巴克莱·艾尔斯
    public class BarclayEls : CardEffect
    {
        //从牌组打出1张随机铜色/银色矮人牌，并使其获得3点强化。
        public BarclayEls(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deck = Game.PlayersDeck[PlayerIndex];
            var myId = Card.CardInfo().CardId;
            var cardsToPlay = deck.Where(x => x.IsAnyGroup(Group.Copper, Group.Silver) && x.CardInfo().Categories.Contains((Categorie.Dwarf)));
            var list = cardsToPlay.Mess(RNG).ToList();
            if (!list.Any()) return 0;
            var card = list.First();
            await card.Effect.Strengthen(3, Card);
            await card.MoveToCardStayFirst();
            return 1;
        }
    }
}