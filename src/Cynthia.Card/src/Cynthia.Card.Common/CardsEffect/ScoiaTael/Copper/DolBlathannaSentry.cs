using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54027")]//多尔·布雷坦纳哨兵
	public class DolBlathannaSentry : CardEffect
	{//位于己方半场、牌组或手牌：己方打出特殊牌时获得1点增益。
		public DolBlathannaSentry(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}