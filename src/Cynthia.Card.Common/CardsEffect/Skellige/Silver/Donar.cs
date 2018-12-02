using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("63007")]//多纳·印达
	public class Donar : CardEffect
	{//改变1个单位的锁定状态。从对方墓场中1张铜色单位牌移至己方墓场。
		public Donar(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}