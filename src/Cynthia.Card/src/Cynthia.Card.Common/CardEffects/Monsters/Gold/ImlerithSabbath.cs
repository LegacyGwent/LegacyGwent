using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22009")]//伊勒瑞斯：临终之日
	public class ImlerithSabbath : CardEffect
	{//2点护甲。 每回合结束时，与最强的敌军单位对决。若存活，则回复2点战力并获得2点护甲。
		public ImlerithSabbath(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}