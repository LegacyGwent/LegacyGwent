using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70020")]//菲吉斯·梅鲁佐
    public class FiggisMerluzzo : CardEffect
    {//召唤“卓尔坦·矮人战士”和“穆罗·布鲁伊斯”，并使其获得等同于自身增益点数一半的增益
        public FiggisMerluzzo(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (true)
            {

            }
            var myDeck = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersDeck[PlayerIndex]).ToList();
            var MunroBruyss = myDeck.Where(x => x.Status.CardId == CardId.MunroBruys).ToList();
            var ZoltanWarriors = myDeck.Where(x => x.Status.CardId == CardId.ZoltanWarrior).ToList();
            var targetcard = myDeck.Where(x => x.Status.CardId == CardId.MunroBruys || x.Status.CardId == CardId.ZoltanWarrior).ToList();
            var point = (Card.CardPoint() - Card.Status.Strength) / 2;
            foreach (var MunroBruys in MunroBruyss)
            {
                await MunroBruys.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach (var ZoltanWarrior in ZoltanWarriors)
            {
                await ZoltanWarrior.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach (var Cards in targetcard)
            {
                await Cards.Effect.Boost(point, Card);
            }
            return 0;
        }
    }
}