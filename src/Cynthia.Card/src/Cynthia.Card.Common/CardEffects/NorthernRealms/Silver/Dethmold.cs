using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43013")]//戴斯摩
	public class Dethmold : CardEffect
	{//生成“倾盆大雨”、“晴空”或“阿尔祖落雷术”。
		public Dethmold(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}