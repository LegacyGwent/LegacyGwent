using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70174")]//
    public class MageInfiltrator : CardEffect
    {//间谍。揭示2张敌方手牌，选择1个敌军铜色单位或1张被揭示的敌方铜色单位牌，生成其佚亡原始同名牌。
        public MageInfiltrator(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {   // Reveal two enemy cards
            var list = Game.PlayersHandCard[Card.PlayerIndex]
                .Where(x => !x.Status.IsReveal).ToList();
            var cards = await Game.GetSelectMenuCards(Game.AnotherPlayer(Card.PlayerIndex), list, 2, isEnemyBack: true);
            foreach (var card in cards)
            {
                await card.Effect.Reveal(Card);
            }
            var controller = Game.AnotherPlayer(Card.PlayerIndex);
            var candidates = Game.GetPlaceCards(Card.PlayerIndex)
                .Concat(Game.PlayersHandCard[Card.PlayerIndex].Where(x => x.Status.IsReveal))
                .Where(x => x.Status.Type == CardType.Unit &&
                            x.Status.Group == Group.Copper &&
                            !x.Status.IsSpying &&
                            x.Status.CardId != Card.Status.CardId)
                .Distinct()
                .ToList();
            var selected = await Game.GetSelectMenuCards(controller, candidates, isCanOver: true);
            if (!selected.TrySingle(out var targetCard)) return 0;
            await Game.CreateCard(
                targetCard.Status.CardId,
                controller,
                new CardLocation(RowPosition.MyStay, 0),
                status => status.IsDoomed = true);
            return 1;
        }
    }
}
