using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53013")]//艾达·艾敏
	public class IdaEmeanAepSivney : CardEffect
	{//生成“蔽日浓雾”、“晴空”或“阿尔祖落雷术”。
		public IdaEmeanAepSivney(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}