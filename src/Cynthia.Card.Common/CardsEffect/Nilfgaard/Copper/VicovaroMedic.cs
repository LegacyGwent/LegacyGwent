using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34030")]//维可瓦罗医师
	public class VicovaroMedic : CardEffect
	{//从对方墓场复活1个铜色单位。
		public VicovaroMedic(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}