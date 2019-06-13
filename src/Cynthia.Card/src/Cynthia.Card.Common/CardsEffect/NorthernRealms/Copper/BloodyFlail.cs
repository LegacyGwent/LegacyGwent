using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44034")]//染血连枷
	public class BloodyFlail : CardEffect
	{//造成5点伤害，并在随机排生成1只“鬼灵”。
		public BloodyFlail(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}