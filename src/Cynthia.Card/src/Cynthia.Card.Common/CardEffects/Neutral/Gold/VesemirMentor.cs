using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12023")]//维瑟米尔：导师
	public class VesemirMentor : CardEffect
	{//从牌组打出1张铜色/银色“炼金”牌。
		public VesemirMentor(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			//己方卡组乱序呈现
            var list = Game.PlayersDeck[PlayerIndex]
				.Where(x=>(x.Status.Categories.Contains(Categorie.Alchemy)
					&&(x.Status.Group==Group.Copper||x.Status.Group==Group.Silver)))
				.Mess()
				.ToList();
            //让玩家选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(PlayerIndex, list);
            if (result.Count == 0) return 0;//如果没有任何符合标准的牌,返回
            await result.Single().MoveToCardStayFirst();
			return 1;
		}
	}
}