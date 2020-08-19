using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70027")]//杰洛特：亚克席法印
    public class GeraltAxii : CardEffect
    {//重新打出对方场上的一张非领袖非同名忠诚单位卡，随后将其移回对方半场。

        public GeraltAxii(GameCard card) : base(card) { }

        private GameCard _targetCard = null;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, filter: (x => (x.Status.Group != Group.Leader) || x.Status.CardId != Card.Status.CardId && x.CardInfo().CardUseInfo == CardUseInfo.MyRow), selectMode: SelectModeType.EnemyRow);

            if (!result.TrySingle(out var targetCard))
            {
                return 0;
            }

            targetCard.Effect.Repair(true);
            await targetCard.MoveToCardStayFirst(true);
            // await PlayStayCard(1, false);
            // await targetCard.Effect.Charm(TargetCard);
            _targetCard = targetCard;

            return 1;
        }

        //临时效果
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (_targetCard == null)
            {
                return;
            }
            await _targetCard.Effect.Charm(Card);
        }
    }
}
