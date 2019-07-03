using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64001")]//奎特家族战士
	public class AnCraiteWarrior : CardEffect
	{//对自身造成1点伤害。
		public AnCraiteWarrior(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}