using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70176")]//杂交兽
    public class Hybrid : CardEffect, IHandlesEvent<AfterCardDeath>
    {//Deploy: Boost a non-Gold Beast in your deck by 2. Deathwish: Boost the smallest non-Gold Beast in your deck by 4.
    // 部署：使卡组中的一个野兽单位获得2点增益。遗愿：使卡组中战力最低的野兽获得4点增益。
        public Hybrid(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deck_list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit && x.Status.Group != Group.Gold).FilterCards(filter: x => x.HasAllCategorie(Categorie.Beast)) //Select a bronze or silver beast in your deck
                .Mess(Game.RNG)
                .ToList();
            if (deck_list.Count() == 0)
            {
                return 0;
            }
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, deck_list, 1, "选择1张卡牌");

            if (cards.Count() == 0)
            {
                return 0;
            }
        
            foreach (var card in cards)
            {
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
        // Deathwish: Boost the smallest non-Gold Beast in your deck by 4.
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            //Select the lowest bronze or silver beast in your deck
            var mybeasts = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit && x.Status.Group != Group.Gold).FilterCards(filter: x => x.HasAllCategorie(Categorie.Beast)).WhereAllLowest()
                .Mess(Game.RNG)
                .ToList();
            if (mybeasts.Count() == 0)
            {
                return;
            }
            await mybeasts.Mess(Game.RNG).First().Effect.Boost(4, Card);
            return;
        }
    }
}