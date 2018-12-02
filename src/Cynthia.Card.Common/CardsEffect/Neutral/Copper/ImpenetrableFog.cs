using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14011")]//蔽日浓雾
	public class ImpenetrableFog : CardEffect
	{//在对方单排降下灾厄。回合开始时，对所在排最强的单位造成2点伤害。
		public ImpenetrableFog(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}