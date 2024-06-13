using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("13023")] //希姆
    public class BlackBlood : Choosespell
    {
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public BlackBlood(GameCard card) : base(card)
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
                {1, "BlackBlood_1_CreateVampire"},
                {2, "BlackBlood_2_DestroyVampire"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var cards = GwentMap.GetCreateCardsId(x => x.Group == Group.Copper &&
                    (x.Categories.Contains(Categorie.Necrophage) ||
                    x.Categories.Contains(Categorie.Vampire)), Game.RNG).ToArray();
            if ((await Game.CreateAndMoveStay(PlayerIndex, cards, isCanOver: true)) == 1)
            {
                await Game.PlayersStay[PlayerIndex].First().Effect.Boost(2, Card);
                return 1;
            }
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            var target = await Game.GetSelectPlaceCards(Card, 1, false,
            x => x.HasAnyCategorie(Categorie.Necrophage, Categorie.Vampire) && x.IsAnyGroup(Group.Copper, Group.Silver));
            if (target.Count == 0) return 0;
            await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;
        }
    }
}