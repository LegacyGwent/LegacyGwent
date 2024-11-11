using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70003")] //哈蒙德
    public class Hammond : Choose, IHandlesEvent<BeforeCardDamage>
    {//己方半场同排单位免疫来自灾厄的伤害。择一：生成一张史凯利格铜色机械单位；或使战场上所有友方机械获得3点强化。
        private const int StrengthenPoint = 2;
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public Hammond(GameCard card) : base(card)
        {
        }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await FUNCTION1();
                case 2:
                    return await FUNCTION2();
            }

            return 0;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "Hammond_1_SpawnMachine"},
                {2, "Hammond_2_BoostMachines"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Unit,
                x => x.HasAllCategorie(Categorie.Machine), Faction.Skellige)
                .Select(x => x.CardId);
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
        }

        private async Task<int> FUNCTION2()
        {
            var targets = Game.GetPlaceCards(PlayerIndex).Where(x => x != Card && x.HasAllCategorie(Categorie.Machine)).ToList();
            foreach (var target in targets)
            {
                await target.Effect.Strengthen(StrengthenPoint, Card);
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
