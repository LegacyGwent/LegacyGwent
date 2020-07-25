using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70005")]//科沃的维索戈塔
    public class VysogotaOfCorvo : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardDeath>
    {//回合开始时，左侧单位获得3点增益，自身受到1点伤害，并移至己方单位最少排。遗愿：己方场上最弱单位获得6点增益。
        public VysogotaOfCorvo(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }

            var list = Game.GetAllPlaceCards().Where(x => x.PlayerIndex == PlayerIndex).ToList();

            if (list.Count() <= 0)
            {
                return;
            }

            var target = list.WhereAllLowest().Mess(RNG).First();
            await target.Effect.Boost(6, Card);

            return;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var rowIndex = Card.GetLocation(Card.PlayerIndex).CardIndex;
            var target = Card.GetRangeCard(1, GetRangeType.HollowLeft);

            if (target.Count > 0)
            {
                await target.Single().Effect.Boost(3, Card);
            }

            await Damage(1, Card);
            var row = Game.PlayersPlace[PlayerIndex].Indexed().OrderBy(x => x.Value.Count).First().Key.IndexToMyRow();
            if (row != Card.Status.CardRow)
            {
                await Move(new CardLocation(row, int.MaxValue), Card);
            }
        }
    }
}