using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53010")]//布蕾恩
	public class Braenn : CardEffect
	{//对1个敌军单位造成等同于自身战力的伤害。若目标被摧毁，则使位于手牌、牌组和己方半场除自身外所有“树精”和伏击单位获得1点增益。
		public Braenn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}