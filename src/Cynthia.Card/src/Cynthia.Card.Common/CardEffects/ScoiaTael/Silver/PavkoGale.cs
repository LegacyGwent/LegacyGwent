using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53012")]//帕扶科“舅舅”盖尔
	public class PavkoGale : CardEffect
	{//从牌组打出1张铜色/银色“道具”牌。
		public PavkoGale(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}