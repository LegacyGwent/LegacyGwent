using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13011")]//卡罗“砍刀”凡瑞西
	public class Cleaver : CardEffect
	{//造成等同于手牌数量的伤害。
		public Cleaver(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}