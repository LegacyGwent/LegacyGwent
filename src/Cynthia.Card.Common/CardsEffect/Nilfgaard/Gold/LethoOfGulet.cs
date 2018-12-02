using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("32014")]//古雷特的雷索
	public class LethoOfGulet : CardEffect
	{//间谍。改变同排2个单位的锁定状态，随后汲食它们的所有战力。
		public LethoOfGulet(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}