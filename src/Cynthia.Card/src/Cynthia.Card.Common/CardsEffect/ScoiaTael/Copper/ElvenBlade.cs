using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54031")] //精灵利剑
    public class ElvenBlade : CardEffect
    {
        //对1个非“精灵”单位造成10点伤害。
        public ElvenBlade(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task<int> CardUseEffect()
        {
            var list = await Game.GetSelectPlaceCards(Card, Sizer: isNotElf);
            if (list.Count <= 0) return 0;
            var card = list.Single();
            await card.Effect.Damage(damage);
            if (card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(damage);
            }

            return 0;
        }

        private bool isNotElf(GameCard card)
        {
            return !card.CardInfo().Categories.Contains(Categorie.Elf);
        }

        private const int damage = 10;
    }
}