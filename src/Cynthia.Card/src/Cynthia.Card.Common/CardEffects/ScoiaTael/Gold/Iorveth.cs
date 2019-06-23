using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52007")]//伊欧菲斯
	public class Iorveth : CardEffect
	{//对1个敌军单位造成8点伤害。若目标被摧毁，则使手牌中所有“精灵”单位获得1点增益。
		public Iorveth(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}