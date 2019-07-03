using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42012")]//弗农·罗契
	public class VernonRoche : CardEffect
	{//对1个敌军单位造成7点伤害。 对局开始时，将1个“蓝衣铁卫突击队”加入牌组。
		public VernonRoche(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}