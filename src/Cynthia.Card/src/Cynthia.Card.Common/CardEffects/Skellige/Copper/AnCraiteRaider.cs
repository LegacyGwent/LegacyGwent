using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64024")]//奎特家族突袭者
	public class AnCraiteRaider : CardEffect
	{//被丢弃时复活自身。
		public AnCraiteRaider(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}