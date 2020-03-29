using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70003")]//哈蒙德
    public class Hammond : CardEffect, IHandlesEvent<BeforeCardDamage>
    {//己方半场同排单位免疫来自灾厄的伤害。择一：生成一张史凯利格铜色机械单位；或使战场上所有友方机械获得3点强化。
        public Hammond(GameCard card) : base(card) { }
        private const int strengthenPoint = 2;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("奇兵", "生成一张史凯利格铜色机械单位。"),
                ("号令", "或使战场上所有友方机械获得3点强化。")
            );
            if (switchCard == 0)
            {
                var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit,
                    x => x.HasAllCategorie(Categorie.Machine), Faction.Skellige)
                    .Select(x => x.CardId);
                return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
            }
            else if (switchCard == 1)
            {
                var targets = Game.GetPlaceCards(PlayerIndex).Where(x => x != Card && x.HasAllCategorie(Categorie.Machine)).ToList();

                foreach (var target in targets)
                {
                    await target.Effect.Strengthen(strengthenPoint, Card);
                }
            }
            return 0;
        }

        public async Task HandleEvent(BeforeCardDamage @event)
        {
            //不在场上，返回
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var currentRow = Card.Status.CardRow;
            if (@event.DamageType.IsHazard() && @event.Target.Status.CardRow == currentRow && @event.Target.PlayerIndex == Card.PlayerIndex)
            {
                @event.IsCancel = true;
            }

            await Task.CompletedTask;
            return;
        }
    }
}