using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13010")]//奥克维斯塔
	public class Ocvist : CardEffect
	{//力竭。 4回合后的回合开始时：对所有敌军单位造成1点伤害，随后返回手牌。
		public Ocvist(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}