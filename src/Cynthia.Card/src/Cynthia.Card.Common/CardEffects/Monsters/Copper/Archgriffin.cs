using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24004")]//大狮鹫
	public class Archgriffin : CardEffect
	{//移除所在排的灾厄。
		public Archgriffin(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<NoneStatus>();
            return 0;
		}
	}
}