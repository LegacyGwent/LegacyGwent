using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70091")]//海之歌者
    pubic class HavfrueSinger : CardEffect
    {//若位于倾盆大雨或史凯利格风暴中，将两个敌方单位移至敌方半场同排。
        public HavfrueSinger(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if ((Game.GameRowEffect[Card.PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.SkelligeStorm) || (Game.GameRowEffect[Card.PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.TorrentialRain))
            {
                var selectCard = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.CardRow != Card.Status.CardRow);
                foreach (var target in selectCard)
                {
                    await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
                }
            }
            return 0;
        }
    }
}
