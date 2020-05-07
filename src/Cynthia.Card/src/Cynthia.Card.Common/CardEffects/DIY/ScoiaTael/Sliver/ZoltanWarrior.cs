using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70019")]//卓尔坦·矮人战士
    public class ZoltanWarrior : CardEffect
    {//召唤“菲吉斯·梅鲁佐”和“穆罗·布鲁伊斯”，并将自身战力应用于这两个单位。
        public ZoltanWarrior(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersDeck[PlayerIndex]).ToList();
            var FiggisMerluzzos = myDeck.Where(x => x.Status.CardId == CardId.FiggisMerluzzo).ToList();
            var MunroBruyss = myDeck.Where(x => x.Status.CardId == CardId.MunroBruys).ToList();
            var targetcard = myDeck.Where(x => x.Status.CardId == CardId.MunroBruys || x.Status.CardId == CardId.FiggisMerluzzo).ToList();
            var point = Card.CardPoint()-Card.Status.Strength;
            
            foreach (var FiggisMerluzzo in FiggisMerluzzos)
            {
                await FiggisMerluzzo.Effect.Summon(Card.GetLocation()+ 1, Card);
            }
            foreach (var MunroBruys in MunroBruyss)
            {
                await MunroBruys.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach(var Cards in targetcard)
            {
                await Cards.Effect.Boost(point, Card);
            }
            return 0;
        }
    }
}