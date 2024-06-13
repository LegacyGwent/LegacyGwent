using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("32011")] //希姆
    public class LethoKingslayer : Choose
    {
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public LethoKingslayer(GameCard card) : base(card)
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
                {1, "LethoKingslayer_1_Destroy"},
                {2, "LethoKingslayer_2_PlayTactic"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            //1的效果
            var target = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.Group == Group.Leader && x.PlayerIndex != Card.PlayerIndex);
            if (target.Count == 0) return 0;
            await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            await Boost(5, Card);
            return 1;
        }

        private async Task<int> FUNCTION2()
        {
           //2的效果
            var cards = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.Status.Categories.Contains(Categorie.Tactic))
                .ToList();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards(PlayerIndex, cards);
            if (result.Count() == 0) return 0;
            await result.First().MoveToCardStayFirst();
            return 1;
        }
    }
}