using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54023")] //黑豹
    public class Panther : CardEffect
    {
        //若对方某排单位少于4个，则对其中1个单位造成7点伤害。
        public Panther(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow, filter: Filter);
            if (!cards.Any()) return 0;
            var card = cards.First();
            await card.Effect.Damage(damage,Card);
            return 0;
        }

        private const int damage = 7;

        private bool Filter(GameCard card)
        {
            var location = card.GetLocation();
            var row = Game.RowToList(Card.PlayerIndex, location.RowPosition).ToList();

            return row.Count < 4;
        }
    }
}