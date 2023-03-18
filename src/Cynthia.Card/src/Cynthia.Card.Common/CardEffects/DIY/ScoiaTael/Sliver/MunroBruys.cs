using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70021")]//穆罗·布鲁伊斯
    public class MunroBruys : CardEffect, IHandlesEvent<AfterCardStrengthen>
    {//召唤“卓尔坦·矮人战士”和“菲吉斯·梅鲁佐”，并使其获得等同于自身增益点数一半的增益
        public MunroBruys(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersDeck[PlayerIndex]).ToList();
            var FiggisMerluzzos = myDeck.Where(x => x.Status.CardId == CardId.FiggisMerluzzo).ToList();
            var ZoltanWarriors = myDeck.Where(x => x.Status.CardId == CardId.ZoltanWarrior).ToList();
            var targetcard = myDeck.Where(x => x.Status.CardId == CardId.FiggisMerluzzo || x.Status.CardId == CardId.ZoltanWarrior).ToList();
            foreach (var FiggisMerluzzo in FiggisMerluzzos)
            {
                await FiggisMerluzzo.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            foreach (var ZoltanWarrior in ZoltanWarriors)
            {
                await ZoltanWarrior.Effect.Summon(Card.GetLocation() + 1, Card);
            }

            return 0;
        }

        private async Task StrengthenMyself(GameCard target, GameCard source)
        {
            if (target == Card && source != Card && Card.Status.CardRow.IsOnPlace())
                await StrengthenMyself();
        }

        private async Task StrengthenMyself()
        {
            await Card.Effect.Strengthen(1, Card);
        }
        
        public async Task HandleEvent(AfterCardStrengthen @event)
        {
            await StrengthenMyself(@event.Target, @event.Source);
        }
        
    }
}








