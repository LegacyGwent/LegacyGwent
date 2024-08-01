using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70167")]//卓尔坦的伙伴 ZoltansCompany
    public class ZoltansCompany : CardEffect, IHandlesEvent<AfterUnitPlay>
    {//put back up to 3 non-gold dwarves in your deck THEN play a non-gold dwarf from your deck. Whenever this card is in the graveyard, give 1 armor to each dwarf you play
        public ZoltansCompany(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var Rlist = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.HasAnyCategorie(Categorie.Dwarf) && x.CardInfo().CardType == CardType.Unit);
            if (Rlist.Count() == 0) 
            {
                return 0;
            }            
            var Rresult = await Game.GetSelectMenuCards(Card.PlayerIndex, Rlist.ToList(), 3, "选则放回三张牌");
            foreach (var card in Rresult)
            {
                card.Effect.Repair();
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), card);
            }
            var Plist = Game.PlayersDeck[Card.PlayerIndex].Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Dwarf)).Mess(RNG);
            var Presult = await Game.GetSelectMenuCards(Card.PlayerIndex, Plist.ToList(), 1, "选择打出一张牌");
            if (Presult.Count() == 0) return 0;
            await Presult.First().MoveToCardStayFirst();
            return 1;
        }
        public async Task HandleEvent(AfterUnitPlay @event)
            {
                if (@event.PlayedCard.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInCemetery() && @event.PlayedCard != Card )
                {
                    if (@event.PlayedCard.HasAnyCategorie(Categorie.Dwarf))
                    {
                        await @event.PlayedCard.Effect.Armor(1, Card);
                    }
                }
                return;
            }
    }
    
}
