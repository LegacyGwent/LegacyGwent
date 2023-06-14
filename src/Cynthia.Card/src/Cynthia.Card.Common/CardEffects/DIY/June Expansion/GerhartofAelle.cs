using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70087")]//艾勒的格哈特
    public class GerhartofAelle : CardEffect
    {//play a bronze or silver mage from your deck, or generate and play a bronze spell
        public GerhartofAelle(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {//For some reason when I change the text server breaks, as this game doesn't appear in-game, I left it as-is
            var switchCard = await Card.GetMenuSwitch(
                ("寄生之缚", "从牌组打出1张铜色/银色“诅咒生物”牌"),
                ("低语", "创造对方初始牌组中1张银色单位牌")
           );

            //play a bronze or silver mage from your deck
            if (switchCard == 0)
            {
                //select mages from your deck
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Mage) &&
                       (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper))
                    .Mess(Game.RNG)
                    .ToList();

                if (list.Count() == 0)
                {
                    return 0;
                }
                //choose mage to play
                var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
                if (cards.Count() == 0)
                {
                    return 0;
                }
                var playCard = cards.Single();
                await playCard.MoveToCardStayFirst();
                return 1;
            }

            //generate and play a bronze spell
            else if (switchCard == 1)
            {

                var cardsId = GwentMap.GetCards().FilterCards(Group.Copper, CardType.Special, x => x.HasAllCategorie(Categorie.Spell))
                    .Select(x => x.CardId);

                return await Game.CreateAndMoveStay(PlayerIndex, cardsId.ToArray());
            }

            return 0;
        }
    }
}