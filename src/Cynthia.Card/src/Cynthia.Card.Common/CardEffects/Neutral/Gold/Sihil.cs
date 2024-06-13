using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("12042")] //希姆
    public class Sihil : Choosespell3
    {
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public Sihil(GameCard card) : base(card)
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
                case 3:
                    return await FUNCTION3();
            }

            return 0;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "Sihil_1_DamageOdd"},
                {2, "Sihil_2_DamageEven"},
                {3, "Sihil_3_PlayUnit"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.CardPoint() % 2 == 1).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {

                await card.Effect.Damage(3, Card);
            }
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.CardPoint() % 2 == 0).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {

                await card.Effect.Damage(3, Card);
            }
            return 0;
        }
        private async Task<int> FUNCTION3()
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit)).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
            if (!list.TryMessOne(out var target, Game.RNG))
            {
                return 0;
            }
            await target.MoveToCardStayFirst();
            return 1;
        }
    }
}