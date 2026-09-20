using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70193")]// necromancer tome
    public class NecromancerTome : CardEffect
    {//Destroy up to 4 friendly units. Spawn a Specter in their place.
    // 摧毁4个友方单位，并在原位各生成1个"鬼灵"。

        public NecromancerTome(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            for (var i = 0; i < 4; i++)
            {
                var card = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow)).SingleOrDefault();
                if (card != default)
                {
                    var position = card.GetLocation();
                    await card.Effect.ToCemetery(CardBreakEffectType.Epidemic);
                    await Game.CreateCard(CardId.Specter, PlayerIndex, position);
                }
            }
            return 0;
        }
    }
}