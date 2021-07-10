using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;
namespace Cynthia.Card
{
    [CardEffectId("70076")]//集会传教士 CongregationCleric
    public class CongregationCleric : CardEffect, IHandlesEvent<AfterCardLock>
    {//在同排召唤所有被锁定的铜色单位的2战力的原始同名牌。己方回合中，每当铜色单位被锁定，在同排召唤其2战力的原始同名牌。
        public CongregationCleric(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //对方全场的锁定铜单位
            var lockList = Game.GetPlaceCards(PlayerIndex).Concat(Game.GetPlaceCards(AnotherPlayer))
            .FilterCards(filter: x => x.Status.Group==Group.Copper && x.Status.IsLock == true).ToList();
            foreach(var card in lockList )
            {
                await Game.CreateCardAtEnd(card.CardInfo().CardId, PlayerIndex, Card.Status.CardRow, setting: Lesser);
            }
            return 0;
        }
        public async Task HandleEvent(AfterCardLock @event)
        {
            if ((@event.Source.PlayerIndex == PlayerIndex) && (Card.Status.CardRow.IsOnPlace()))
            {
                await Game.CreateCardAtEnd(@event.Target.CardInfo().CardId, PlayerIndex, Card.Status.CardRow, setting: Lesser);
            }
            return;
        }
        private void Lesser(CardStatus status)
        {
            status.IsDoomed = true;
            status.Strength = 2;
        }
    }
    
}