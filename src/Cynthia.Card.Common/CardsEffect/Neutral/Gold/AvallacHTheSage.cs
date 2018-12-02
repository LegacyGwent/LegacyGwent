using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12029")]//阿瓦拉克：贤者
	public class AvallacHTheSage : CardEffect
	{//随机生成1张对方起始牌组中金色/银色单位牌的原始同名牌。
		public AvallacHTheSage(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}