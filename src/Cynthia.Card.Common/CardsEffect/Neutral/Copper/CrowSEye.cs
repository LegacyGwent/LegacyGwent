using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14021")]//乌鸦眼
	public class CrowSEye : CardEffect
	{//对对方每排最强的敌军单位造成4点伤害。己方墓场每有1张同名牌，便使伤害提高1点。
		public CrowSEye(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			//var damage = 4+(Game.PlayersCemetery[Card.PlayerIndex]);
			var count = Game.PlayersDeck[Card.PlayerIndex].Count<3?Game.PlayersDeck[Card.PlayerIndex].Count:3;
			var list = Game.PlayersDeck[Card.PlayerIndex].ToList();
			for(var i = 0; i<count; i++)
			{
				await list[i].MoveToCardStayFirst();
			}
			return count;
		}
	}
}