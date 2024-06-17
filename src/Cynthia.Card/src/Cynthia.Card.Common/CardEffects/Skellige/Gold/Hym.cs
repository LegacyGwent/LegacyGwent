using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("62012")] //希姆
    public class Hym : Choose
    {
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public Hym(GameCard card) : base(card)
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
                {1, "Hym_1_PlayCursed"},
                {2, "Hym_2_PlaySilver"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            //乱序列出诅咒生物，如果没有，什么都不做
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Cursed) &&
                   (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                .Mess(Game.RNG)
                .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
            //选一张，如果没选，什么都不做
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
            if (cards.Count() == 0)
            {
                return 0;
            }

            //打出
            var playCard = cards.Single();
            await playCard.MoveToCardStayFirst();
            return 1;
        }

        private async Task<int> FUNCTION2()
        {
            //手动排除大间谍
            var cardsId = Game.PlayerBaseDeck[AnotherPlayer].Deck
               .Select(x => x.CardId)
               .Distinct()
               .Where(x => !GwentMap.CardMap[x].HasAnyCategorie(Categorie.Agent) && GwentMap.CardMap[x].Is(Group.Silver, CardType.Unit))
               .Mess(Game.RNG)
               .Take(3).ToArray();
            return await Game.CreateAndMoveStay(PlayerIndex, cardsId);
        }
    }
}
