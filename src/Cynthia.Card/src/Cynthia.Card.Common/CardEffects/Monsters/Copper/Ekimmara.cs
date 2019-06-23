using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24026")]//蝙魔
	public class Ekimmara : CardEffect
	{//汲食1个单位3点战力。
		public Ekimmara(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}