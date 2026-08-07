using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("44033")]//绞盘
    public class Winch : Choosespell
    {//择一：复活1个铜色机械并使其获得佚亡；使所有友军机械获得3点增益。
        public Winch(GameCard card) : base(card) { }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await ResurrectMachine();
                case 2:
                    return await BoostMachines();
                default:
                    return 0;
            }
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "Winch_1_Resurect"},
                {2, "Winch_2_Boost"}
            };
        }

        private async Task<int> ResurrectMachine()
        {
            var candidates = Game.PlayersCemetery[PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper &&
                            x.CardInfo().CardType == CardType.Unit &&
                            x.HasAnyCategorie(Categorie.Machine))
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, candidates, 1, "选择复活一张牌"))
                .TrySingle(out var target))
            {
                return 0;
            }

            target.Status.IsDoomed = true;
            await target.Effect.Resurrect(
                new CardLocation { RowPosition = RowPosition.MyStay, CardIndex = 0 },
                Card);
            return 1;
        }

        private async Task<int> BoostMachines()
        {
            var machines = Game.GetPlaceCards(PlayerIndex)
                .FilterCards(filter: x => x.HasAllCategorie(Categorie.Machine) &&
                                         x.CardInfo().CardType == CardType.Unit);
            foreach (var machine in machines)
            {
                await machine.Effect.Boost(3, Card);
            }
            return 0;
        }
    }
}
