using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54019")]//烟火技师
	public class Pyrotechnician : CardEffect
	{//对对方每排的1个随即单位造成3点伤害。
		public Pyrotechnician(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}