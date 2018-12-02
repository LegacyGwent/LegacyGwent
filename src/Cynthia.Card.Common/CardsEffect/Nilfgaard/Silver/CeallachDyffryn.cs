using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("33017")]//契拉克·迪弗林
	public class CeallachDyffryn : CardEffect
	{//生成1个“大使”、“刺客”或“特使”。
		public CeallachDyffryn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}