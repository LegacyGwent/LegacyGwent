using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62004")]//莫斯萨克
	public class Ermion : CardEffect
	{//抽2张牌，随后丢弃2张牌。
		public Ermion(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}