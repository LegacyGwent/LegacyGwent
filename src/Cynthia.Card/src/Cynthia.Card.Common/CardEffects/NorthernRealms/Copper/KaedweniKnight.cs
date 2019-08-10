using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44009")]//科德温骑士
	public class KaedweniKnight : CardEffect
	{//若从牌组打出，则获得5点增益。 2点护甲。
		public KaedweniKnight(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			 return 0;
		}
	}
}