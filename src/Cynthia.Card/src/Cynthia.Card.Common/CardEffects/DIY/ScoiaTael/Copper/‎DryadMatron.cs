using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;


namespace Cynthia.Card
{
    [CardEffectId("70122")]//树精族母 DryadMatron
    public class DryadMatron : CardEffect
    {//随机使牌组中战力最低的单位牌获得2点增益，若为树精则改为2点强化。
        public DryadMatron(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[PlayerIndex]
            .Where(x => (x.CardInfo().CardType == CardType.Unit)).WhereAllLowest().ToList();
            if (list.Count() == 0) return 0;
            var cards = list.Mess(RNG).First();
            if(cards.HasAllCategorie(Categorie.Dryad))
            {
                await cards.Effect.Strengthen(2, cards);
                return 0;
            }
            await cards.Effect.Boost(2, cards);
            return 0;
        }
        
       
    }
}
