using System.Threading.Tasks;
using System.Linq;

namespace Cynthia.Card
{
    [CardEffectId("70191")]//奥菲莉亚·凡·莫拉汉姆 Ophelie Van Moorlehem
    public class OphelieVanMoorlehem : CardEffect
    {//隐藏1张手牌。若为“吸血鬼"牌，则获得其战力一半的增益。
    // Deploy: Conceal a card. If it was a Vampire, boost self by half of its power.
        public OphelieVanMoorlehem (GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            
            var cards = Game.GetAllCard(PlayerIndex).Where(x => x.Status.IsReveal).ToList();
            if (cards.Count == 0)
            {
                return 0;
            }
            var targetCard = await Game.GetSelectMenuCards(PlayerIndex, cards, 1);
            var selectCard = targetCard.Single();
            if (targetCard.Count == 0)
            {
                return 0;
            }
            await selectCard.Effect.Conceal(Card);
            if (selectCard.HasAllCategorie(Categorie.Vampire))
            {
                var boostValue = selectCard.CardPoint() / 2;
                await Card.Effect.Boost(boostValue, Card);
            }
            return 0;
        }
    }
}
