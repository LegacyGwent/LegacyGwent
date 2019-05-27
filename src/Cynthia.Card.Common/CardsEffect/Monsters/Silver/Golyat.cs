using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23003")]//哥亚特
	public class Golyat : CardEffect
	{//获得7点增益。 每次被伤害时，额外承受2点伤害。
		public Golyat(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}