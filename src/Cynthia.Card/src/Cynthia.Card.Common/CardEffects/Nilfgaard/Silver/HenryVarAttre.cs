using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33008")]//亨利·凡·阿特里
	public class HenryVarAttre : CardEffect
	{//隐匿任意数量的单位。若为友军单位，则使它们获得2点增益；若为敌军单位，则对他们造成2点伤害。
		public HenryVarAttre(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}