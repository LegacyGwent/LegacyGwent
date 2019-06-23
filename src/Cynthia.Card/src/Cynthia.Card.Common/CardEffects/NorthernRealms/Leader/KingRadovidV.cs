using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("41001")]//拉多维德五世
	public class KingRadovidV : CardEffect
	{//改变2个单位的锁定状态。若为敌军单位，则对其造成4点伤害。 操控。
		public KingRadovidV(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}