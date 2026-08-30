using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("42007")]//罗契：冷酷之心
    public class RocheMerciless : Choose
    {
        public RocheMerciless(GameCard card) : base(card) { }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await PlayTemeriaUnit();
                case 2:
                    return await DestroyAmbush();
                default:
                    return 0;
            }
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                { 1, "RocheMerciless_1_PlayTemeria" },
                { 2, "RocheMerciless_2_DestroyAmbush" }
            };
        }

        private async Task<int> PlayTemeriaUnit()
        {
            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(card =>
                    card.Status.Categories.Contains(Categorie.Temeria) &&
                    card.CardPoint() <= Card.CardPoint() &&
                    (card.Status.Group == Group.Silver || card.Status.Group == Group.Copper))
                .Mess(Game.RNG)
                .ToList();
            if (!candidates.Any())
            {
                return 0;
            }

            var selected = await Game.GetSelectMenuCards(PlayerIndex, candidates, 1);
            if (!selected.TrySingle(out var playCard))
            {
                return 0;
            }

            await playCard.MoveToCardStayFirst();
            return 1;
        }

        private async Task<int> DestroyAmbush()
        {
            var selected = await Game.GetSelectPlaceCards(
                Card,
                1,
                selectMode: SelectModeType.EnemyRow,
                filter: target => target.Status.Conceal,
                isHasConceal: true);
            if (!selected.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;
        }
    }
}
