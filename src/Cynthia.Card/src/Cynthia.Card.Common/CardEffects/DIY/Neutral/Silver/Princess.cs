using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70172")]//图尔赛克家族驯兽师
    public class Princess : CardEffect, IHandlesEvent<AfterTurnStart>
    {// Deploy: Spawn and Play a Giant Bear. On the start of next turn, transform a Giant Bear on the same row into a Raging Bear.
        public Princess(GameCard card) : base(card) { }
        private bool _isTransformed = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            // Deploy: Spawn and Play a Giant Bear.
            await Game.CreateCard("15010", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            _isTransformed = false;
            return 1;
        }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            // On the start of next turn, transform a Giant Bear on the same row into a Raging Bear.
            if (Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).Any(x => x.Status.CardId == "15011") && !_isTransformed)
            {
                await Game.CreateCard("65002", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
                _isTransformed = true;
            }
        }
    }
}