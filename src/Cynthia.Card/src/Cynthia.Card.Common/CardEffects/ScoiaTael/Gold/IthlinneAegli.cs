using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52011")]//伊斯琳妮
	public class IthlinneAegli : CardEffect
	{//从牌组打出1张铜色“法术”、恩泽或灾厄牌，重复其效果一次。
		public IthlinneAegli(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			//打乱己方卡组,并且选择铜色“法术”、恩泽或灾厄牌
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => (x.Status.Group == Group.Copper) && x.HasAnyCategorie(Categorie.Spell,
			Categorie.Hazard,Categorie.Boon)).Mess(RNG);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
			var selctCardId = result.First().Status.CardId;

            await result.First().MoveToCardStayFirst();
			await Game.CreateToStayFirst(selctCardId, PlayerIndex);
			// await Game.CreateCard(selctCardId,Card.PlayerIndex,new CardLocation(RowPosition.MyStay,0));
			// await Card.CreateAndMoveStay();

            return 2;
		}
	}
}