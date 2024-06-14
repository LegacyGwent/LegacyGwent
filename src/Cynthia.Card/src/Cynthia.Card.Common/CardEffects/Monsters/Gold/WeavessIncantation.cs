using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("22012")]//织婆：咒文
    public class WeavessIncantation : Choose
    {//择一：使位于手牌、牌组和己方半场除自身外的所有“残物”单位获得2点强化；或从牌组打出1张铜色/银色“残物”牌，并使其获得2点强化。
        public WeavessIncantation(GameCard card) : base(card)
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
                {1, "WeavessIncantation_1_Strenghten"},
                {2, "WeavessIncantation_2_PlayRelict"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            //buff牌库
            var listDeck = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Relict)).ToList();

            foreach (var target in listDeck)
            {
                await target.Effect.Strengthen(2, Card);
            }

            //buff手牌
            var listHand = Game.PlayersHandCard[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Relict)).ToList();

            foreach (var target in listHand)
            {
                await target.Effect.Strengthen(2, Card);
            }

            ////buff半场
            var listPlace = Game.GetPlaceCards(Card.PlayerIndex).Where(x => x.Status.Categories.Contains(Categorie.Relict) && x != Card).ToList();
            foreach (var target in listPlace)
            {
                await target.Effect.Strengthen(2, Card);
            }
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Relict)
                    && (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
            .Mess(Game.RNG)
            .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }

            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);

            //没有选就无事发生
            if (cards.Count() == 0)
            {
                return 0;
            }

            //打出
            var playCard = cards.Single();
            await playCard.Effect.Strengthen(2, Card);
            await playCard.MoveToCardStayFirst();
            return 1;
        }
    }
}
