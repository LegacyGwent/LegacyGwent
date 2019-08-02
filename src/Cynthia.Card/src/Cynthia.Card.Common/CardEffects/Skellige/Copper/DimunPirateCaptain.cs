using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64031")]//迪门家族海盗船长
    public class DimunPirateCaptain : CardEffect
    {//从牌组打出1个非同名铜色“迪门家族”单位。
        public DimunPirateCaptain(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //列出非同名迪门家族铜色单位
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.ClanDimun) && (x.Status.Group == Group.Copper) && (x.Status.CardId != Card.Status.CardId)).Mess(Game.RNG).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
			//选一张
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);

            if (cards.Count() == 0)
            {
                return 0;
            }
			//打出
			await cards.Single().MoveToCardStayFirst();

            return 0;


        }
    }
}