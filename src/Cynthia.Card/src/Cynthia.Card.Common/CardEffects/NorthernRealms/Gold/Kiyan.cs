using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("42010")]//凯亚恩
    public class Kiyan : Choose
    {//择一：创造1张铜色/银色“炼金”牌；或从牌组打出1张铜色/银色“道具”牌。
        public Kiyan(GameCard card) : base(card)
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
                {1, "Kiyan_1_CreateAlchemy"},
                {2, "Kiyan_2_PlayItem"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var ids = GwentMap.GetCreateCardsId(x => x.Is(filter: x => x.HasAllCategorie(Categorie.Alchemy) && x.IsAnyGroup(Group.Copper, Group.Silver)), Game.RNG);
            return await Game.CreateAndMoveStay(PlayerIndex, ids.ToArray());
        }

        private async Task<int> FUNCTION2()
        {
           var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Item) &&
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
    }
}
