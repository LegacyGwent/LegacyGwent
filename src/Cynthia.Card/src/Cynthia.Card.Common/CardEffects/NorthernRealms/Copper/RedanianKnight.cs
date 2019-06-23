using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44012")]//瑞达尼亚骑士
	public class RedanianKnight : CardEffect
	{//回合结束时，若不受护甲保护，则获得2点增益和2点护甲。
		public RedanianKnight(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}