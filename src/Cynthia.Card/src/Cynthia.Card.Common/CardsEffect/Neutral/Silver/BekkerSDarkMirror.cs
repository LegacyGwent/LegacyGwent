using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13026")]//贝克尔的黑暗之镜
	public class BekkerSDarkMirror : CardEffect
	{//对场上最强的单位造成最多10点伤害（无视护甲），并使场上最弱的单位获得相同数值的增益。
		public BekkerSDarkMirror(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var list = Game.GetAllCard(PlayerIndex).ToList();
			if(list.Count()<=0) return 0;
			//穿透伤害
			await list.Where(x=>x.Status.CardRow.IsOnPlace()).WhereAllHighest().Mess().First().Effect.Damage(10,Card,BulletType.RedLight,true);
			list = Game.GetAllCard(PlayerIndex).ToList();
			if(list.Count()<=0) return 0;
			await list.Where(x=>x.Status.CardRow.IsOnPlace()).WhereAllLowest().Mess().First().Effect.Boost(10,Card);
			return 0;
		}
	}
}