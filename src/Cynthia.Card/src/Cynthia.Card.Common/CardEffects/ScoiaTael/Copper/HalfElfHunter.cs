using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54017")] //半精灵猎人
    public class HalfElfHunter : CardEffect
    {
        //生成1张佚亡原始同名牌。
        public HalfElfHunter(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var position = Card.GetLocation();
            await Game.CreateCard(Card.Status.CardId, PlayerIndex, position, setting: ToDoomed);
            return 0;
        }

        private void ToDoomed(CardStatus status)
        {
            status.IsDoomed = true;
        }
    }
}