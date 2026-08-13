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
            var enemylist = Game.PlayersCemetery[Game.AnotherPlayer(Card.PlayerIndex)]
                .Where(x => x.CardInfo().CardType == CardType.Unit && x.Status.Group == Group.Copper)
                .ToList();
            if (enemylist.Count == 0)
            {
                return 0;
            }
            var moveTarget = await Game.GetSelectMenuCards(Card.PlayerIndex, enemylist, 1);
            if (moveTarget.Count == 0)
            {
                return 0;
            }
            await Game.ShowCardMove(
                new CardLocation { RowPosition = RowPosition.EnemyCemetery, CardIndex = 0 },
                moveTarget.Single());
            return 0;
		}
	}
}
