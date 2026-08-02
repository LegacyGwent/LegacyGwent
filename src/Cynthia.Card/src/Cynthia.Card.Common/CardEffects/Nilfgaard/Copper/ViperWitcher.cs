using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    public abstract class ViperWitcherEffect : CardEffect
    {
        protected ViperWitcherEffect(GameCard card) : base(card) { }

        protected abstract int GetDamage(int alchemyCount);
        protected virtual bool SkipTargetWhenNoDamage => false;

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var alchemyCount = Card.GetMyBaseDeck(x => x.Categories.Contains(Categorie.Alchemy)).Count;
            var point = GetDamage(alchemyCount);
            if (point <= 0 && SkipTargetWhenNoDamage) return 0;
            var result = await Game.GetSelectPlaceCards(Card);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Damage(point, Card);
            return 0;
        }
    }

    [CardEffectId("34022")]//毒蛇学派猎魔人（Z／原版）
    public class ViperWitcher : ViperWitcherEffect
    {
        public ViperWitcher(GameCard card) : base(card) { }
        protected override int GetDamage(int alchemyCount) => alchemyCount;
    }

    [CardEffectId("34034")]//毒蛇学派猎魔人A
    public class ViperWitcherA : ViperWitcherEffect
    {
        public ViperWitcherA(GameCard card) : base(card) { }
        protected override int GetDamage(int alchemyCount) => alchemyCount / 3 * 2;
        protected override bool SkipTargetWhenNoDamage => true;
    }

    [CardEffectId("34035")]//毒蛇学派猎魔人B
    public class ViperWitcherB : ViperWitcherEffect
    {
        public ViperWitcherB(GameCard card) : base(card) { }
        protected override int GetDamage(int alchemyCount) => 3 + alchemyCount / 3 * 2;
    }

    [CardEffectId("34036")]//毒蛇学派猎魔人C
    public class ViperWitcherC : ViperWitcherEffect
    {
        public ViperWitcherC(GameCard card) : base(card) { }
        protected override int GetDamage(int alchemyCount) => alchemyCount;
    }
}
