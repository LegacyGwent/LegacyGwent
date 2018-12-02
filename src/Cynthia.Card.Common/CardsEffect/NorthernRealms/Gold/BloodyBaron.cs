using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("42002")]//血红男爵
	public class BloodyBaron : CardEffect
	{//若位于手牌、牌组或己方半场：有敌军单位被摧毁时获得1点增益。
		public BloodyBaron(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}