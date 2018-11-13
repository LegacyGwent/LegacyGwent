using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("63003")]//魔像守卫
    public class TheGuardian : CardEffect
    {
        public TheGuardian(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}