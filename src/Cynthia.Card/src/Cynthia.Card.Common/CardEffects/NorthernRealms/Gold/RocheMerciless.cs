using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42007")]//罗契：冷酷之心
    public class RocheMerciless : CardEffect, IHandlesEvent<OnGameStart>
    {//at the beggining of the game, add a blue stripe commando to your deck. Deploy: damage a unit by 2, if it's destroyed, repeat
        public RocheMerciless(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            bool active = true;
            while(active == true)
            { 
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                if (!selectList.TrySingle(out var target))
                {
                    return 0;
                }
                await target.Effect.Damage(2, Card);
                if (target.CardPoint() > 0)
                {
                active = false;
                return 0;    
                }
            }
            return 0;
        }
        public async Task HandleEvent(OnGameStart @event)
        {
            await Game.CreateCard(CardId.BlueStripesCommando, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            return;
        }
    }
}


