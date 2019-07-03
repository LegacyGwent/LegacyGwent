using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34032")]//新兵
    public class Recruit : CardEffect
    {//从牌组随机打出1张非同名“铜色”士兵牌。
        public Recruit(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var targets = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.Categories.Contains(Categorie.Soldier) && x.Status.Group == Group.Copper && x.Status.CardId != CardId.Recruit)
                .Mess();
            if (targets.Count() == 0) return 0;
            await targets.First().MoveToCardStayFirst();
            return 1;
        }
    }
}