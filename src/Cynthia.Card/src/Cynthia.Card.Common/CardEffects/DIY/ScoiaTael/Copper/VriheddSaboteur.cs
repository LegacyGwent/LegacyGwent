using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70098")] //维里赫德旅破坏者 VriheddSaboteu
    public class VriheddSaboteur : CardEffect, IHandlesEvent<BeforeCardDamage>, IHandlesEvent<AfterTurnOver>
    {//随机打出1张铜色道具牌，若牌组数量低于自身战力，改为复活1张铜色道具牌。
        public VriheddSaboteur(GameCard card) : base(card){}
        private int ProDamage = 0;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            ProDamage = 1;
            var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Item) && x.CardInfo().CardType == CardType.Special)).ToList();
            if (list.Count() == 0) return 0;
            var moveCard = list.Mess(RNG).First();
            await moveCard.MoveToCardStayFirst();
            return 1;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if(@event.PlayerIndex == PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                ProDamage = 0;
            }
            
            await Task.CompletedTask;
            return;
        }

        public async Task HandleEvent(BeforeCardDamage @event)
        {
            if (Game.GameRound.ToPlayerIndex(Game) != PlayerIndex || Card.Status.CardRow.IsInDeck() || Card.Status.CardRow.IsInHand() || ProDamage == 0)
            {
                return;
            }
            if (@event.Target.PlayerIndex != Card.PlayerIndex)
            {
                @event.Num = @event.Num + 1;
            }

            await Task.CompletedTask;
            return;
        }
    }
}

















