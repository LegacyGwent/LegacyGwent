using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64015")]//奎特家族盔甲匠
	public class AnCraiteArmorsmith : CardEffect
	{//治愈2个友军单位，并使其获得3点护甲。
		public AnCraiteArmorsmith(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}