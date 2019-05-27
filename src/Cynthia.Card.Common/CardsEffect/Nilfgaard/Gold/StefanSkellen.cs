using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("32004")]//史提芬·史凯伦
	public class StefanSkellen : CardEffect
	{//将牌组任意1张卡牌移至顶端。若它为非间谍单位，则使其获得5点增益。
		public StefanSkellen(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}