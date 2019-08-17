using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54009")] //多尔·布雷坦纳射手
    public class DolBlathannaBowman : CardEffect, IHandlesEvent<AfterCardMove>
    {
        //对1个敌军单位造成2点伤害。 每当有敌军单位改变所在排别，便对其造成2点伤害。 自身移动时对1个敌军随机单位造成2点伤害。
        public DolBlathannaBowman(GameCard card) : base(card)
        {
        }

        private const int damage = 2;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (list.Count <= 0) return 0;
            await list.First().Effect.Damage(damage, Card);
            return 0;
        }

        private GameCard GetEnemyRandomCard()
        {
            var list = Game.GetPlaceCards(Game.AnotherPlayer(Card.PlayerIndex));
            return !list.Any() ? null : list.Mess(Game.RNG).First();
        }

        public async Task HandleEvent(AfterCardMove @event)
        {
            if (@event.Target.PlayerIndex != Card.PlayerIndex)
            {
                await @event.Target.Effect.Damage(damage, Card);
            }

            if (@event.Target == Card)
            {
                var card = GetEnemyRandomCard();
                if (card != null)
                {
                    await card.Effect.Damage(damage, Card);
                }
            }
        }
    }
}