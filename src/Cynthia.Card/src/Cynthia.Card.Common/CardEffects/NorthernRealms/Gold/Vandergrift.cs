using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42004")]//范德格里夫特
	public class Vandergrift : CardEffect
	{//对所有敌军单位造成1点伤害。在被摧毁的单位同排降下“终末之战”。
		public Vandergrift(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}