using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12030")]//狐妖：真身
	public class AguaraTrueForm : CardEffect
	{//不限阵营地创造1张铜色/银色“法术”牌。
		public AguaraTrueForm(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}