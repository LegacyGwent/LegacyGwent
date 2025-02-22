using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("42007")]//罗契：冷酷之心
    public class RocheMerciless : Choose
    {//摧毁1个背面向上的伏击敌军单位
        public RocheMerciless(GameCard card) : base(card)
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
                {1, "RocheMerciless_1_PlayTemeria"},
                {2, "RocheMerciless_2_DestroyAmbush"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Temeria) && x.CardPoint() <= Card.CardPoint() &&
                    (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper)).Mess(Game.RNG).ToList();

            if (list.Count() == 0)
            {
                return 0;
            }

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
           var selectList = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.Conceal == true);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;
        }
    }
}
