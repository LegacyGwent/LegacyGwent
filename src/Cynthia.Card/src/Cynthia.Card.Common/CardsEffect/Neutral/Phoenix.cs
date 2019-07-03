using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12028")]//凤凰
	public class Phoenix : CardEffect
	{//复活1个铜色/银色“龙兽”单位。
		public Phoenix(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var list = Game.PlayersCemetery[PlayerIndex]
	            .Where(x => (x.Categories.Contains(Categorie.Draconid))
	            	&& (x.Status.Group == Group.Silver || x.Status.Group == Group.Silver)).Mess();

            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");

            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0){
            	return 0;
            } 

            //打出
            await result.First().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
			
			return 1;
		}
	}
}