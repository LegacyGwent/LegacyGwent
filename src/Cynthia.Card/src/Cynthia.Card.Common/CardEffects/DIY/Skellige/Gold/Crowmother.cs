using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70159")]//
    public class Crowmother : CardEffect, IHandlesEvent<AfterCardDiscard>
    {//
        public Crowmother(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Special ).Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 3, "选择打出一张牌");
            for (var i = 0; i < result.Count(); i++)
            {
                await result[i].Effect.Discard(Card);
                if (result.Count() == 0)
                {
                    return 0;
                }
            }
            return 0;
        }
        
        public async Task HandleEvent(AfterCardDiscard @event)
        {
            if (Card.Status.CardRow.IsOnPlace() && @event.Target.CardInfo().CardType == CardType.Special
            && (@event.Source.PlayerIndex == Card.PlayerIndex && !@event.Source.HasAnyCategorie(Categorie.Agent)
            || (@event.Source.PlayerIndex != Card.PlayerIndex && @event.Source.HasAnyCategorie(Categorie.Agent))))
            {
                await CrowCreate();
            }
            return;
        }

        private async Task CrowCreate()
        {
            await Game.CreateCard(CardId.Crow, PlayerIndex, Game.GetRandomCanPlayLocation(PlayerIndex, true));
        }
    }
}
