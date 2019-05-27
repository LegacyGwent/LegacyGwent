using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("31003")]//莫尔凡·符里斯
	public class MorvranVoorhis : CardEffect
	{//揭示最多4张牌。
		public MorvranVoorhis(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}