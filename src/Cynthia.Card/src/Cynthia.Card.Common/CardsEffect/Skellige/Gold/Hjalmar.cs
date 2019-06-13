using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62002")]//哈尔玛·奎特
	public class Hjalmar : CardEffect
	{//在对方同排生成“乌德维克之主”。
		public Hjalmar(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}