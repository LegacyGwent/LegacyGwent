using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62006")]//碧尔娜·布兰
	public class BirnaBran : CardEffect
	{//在对方单排降下“史凯利格风暴”。
		public BirnaBran(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}