using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12027")]//狐妖
	public class Aguara : CardEffect
	{//择二：使最弱的友军单位获得5点增益；使手牌中的1个随机单位获得5点增益；对最强的1个敌军单位造成5点伤害；魅惑1个战力不高于5点的敌军“精灵”单位。
		public Aguara(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}