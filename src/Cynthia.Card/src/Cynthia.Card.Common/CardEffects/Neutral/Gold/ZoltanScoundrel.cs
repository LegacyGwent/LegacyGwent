using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12015")]//卓尔坦：流氓
	public class ZoltanScoundrel : CardEffect
	{//择一：生成“话篓子：伙伴”：使2个相邻单位获得2点增益；或生成“话篓子：捣蛋鬼”：对2个相邻单位造成2点伤害。
		public ZoltanScoundrel(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}