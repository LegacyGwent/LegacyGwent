using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33014")]//维尔海夫
    public class Vrygheff : CardEffect
    {//从牌组打出1张铜色“机械”牌。
        public Vrygheff(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Machine)).ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex,list,1);
            if (cards.Count() == 0) return 0;
            await cards.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}