using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43014")]//席儿·德·坦沙维耶
    public class SLeDeTansarville : CardEffect
    {//从手牌打出1张铜色/银色“特殊”牌，随后抽1张牌。
        public SLeDeTansarville(GameCard card) : base(card) { }
        private bool needdraw = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            var handlist = Game.PlayersHandCard[Card.PlayerIndex].Where(x => (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && x.HasAnyCategorie(Categorie.Special));
            if (handlist.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, handlist.ToList(), 1);
            if (result.Count() == 0)
            {
                return 0;
            }
            await result.Single().MoveToCardStayFirst();
            needdraw = true;
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
    }
}