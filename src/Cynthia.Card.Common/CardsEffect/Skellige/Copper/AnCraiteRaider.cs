using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64024")]//奎特家族突袭者
	public class AnCraiteRaider : CardEffect
	{//被丢弃时复活自身。
		public AnCraiteRaider(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}