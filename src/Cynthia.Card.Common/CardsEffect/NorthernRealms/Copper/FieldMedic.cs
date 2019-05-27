using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44005")]//战地医师
	public class FieldMedic : CardEffect
	{//使友军“士兵”单位获得1点增益。
		public FieldMedic(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}