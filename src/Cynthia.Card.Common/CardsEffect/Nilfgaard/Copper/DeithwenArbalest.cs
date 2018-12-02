using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34015")]//戴斯文强弩手
	public class DeithwenArbalest : CardEffect
	{//对1个敌军单位造成3点伤害。若它为间谍，则伤害提升至6点。
		public DeithwenArbalest(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}