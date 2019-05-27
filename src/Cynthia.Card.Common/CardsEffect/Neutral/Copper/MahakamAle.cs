using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14020")]//玛哈坎麦酒
	public class MahakamAle : CardEffect
	{//使己方每排的1个随机单位获得4点增益。
		public MahakamAle(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			foreach(var row in Game.PlayersPlace[Card.PlayerIndex].ToList())
			{
				if(row.Count()!=0)
				{
					await row.Mess().First().Effect.Boost(4,Card);
				}
			}
			return 0;
		}
	}
}