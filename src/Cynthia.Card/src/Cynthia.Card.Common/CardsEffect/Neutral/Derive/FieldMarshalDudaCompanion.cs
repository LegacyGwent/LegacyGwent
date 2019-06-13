using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("15007")]//话篓子：伙伴
	public class FieldMarshalDudaCompanion : CardEffect
	{//使左右各2格内的单位获得2点增益。
		public FieldMarshalDudaCompanion(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}