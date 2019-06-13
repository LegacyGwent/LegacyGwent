using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53004")]//谢尔顿·斯卡格斯
	public class SheldonSkaggs : CardEffect
	{//将同排所有友军单位移至随机排。每移动1个单位，便获得1点增益。
		public SheldonSkaggs(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}