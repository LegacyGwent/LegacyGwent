using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
if (Game.GameRowEffect[Card.PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.SkelligeStorm)|| (Game.GameRowEffect[Card.PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.TorrentialRain)
            {var selectCard = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.CardRow != Card.Status.CardRow);
            foreach (var target in selectCard)
            {
                await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            }
