using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54006")]//先知
	public class Farseer : CardEffect
	{//己方回合中，若有除自身外的友军单位或手牌中的单位获得增益，则回合结束时获得2点增益。
		public Farseer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}