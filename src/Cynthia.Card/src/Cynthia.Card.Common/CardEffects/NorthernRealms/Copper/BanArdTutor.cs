using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44002")]//班·阿德导师
	public class BanArdTutor : CardEffect
	{//用1张手牌交换牌组中的一张铜色“特殊”牌。
		public BanArdTutor(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}