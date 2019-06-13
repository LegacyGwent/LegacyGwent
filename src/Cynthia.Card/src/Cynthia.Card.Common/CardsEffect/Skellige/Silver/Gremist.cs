using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63016")]//格雷密斯特
	public class Gremist : CardEffect
	{//部署：生成“倾盆大雨”、“晴空”或“惊悚咆哮”。
		public Gremist(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}