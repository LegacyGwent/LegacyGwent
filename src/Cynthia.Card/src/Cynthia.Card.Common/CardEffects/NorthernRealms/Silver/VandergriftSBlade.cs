using System.Threading.Tasks;
namespace Cynthia.Card
{
    [CardEffectId("43021")]//范德格里夫特之剑
    public class VandergriftSBlade : CardEffect
    {//造成10点伤害，摧毁铜色/银色诅咒生物，放逐所摧毁的单位。

        public VandergriftSBlade(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selected.TrySingle(out var target))
            {
                return 0;
            }

            var destroyCursed = target.HasAnyCategorie(Categorie.Cursed) &&
                target.IsAnyGroup(Group.Copper, Group.Silver);
            var wasDoomed = target.Status.IsDoomed;
            // Lethal Damage queues cemetery movement. Mark before damage so its
            // eventual death is banished before Deathwish, even outside a task.
            target.Status.IsDoomed = true;
            await target.Effect.Damage(10, Card);
            if (destroyCursed && target.IsAliveOnPlance())
            {
                await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            if (target.IsAliveOnPlance())
            {
                target.Status.IsDoomed = wasDoomed;
                await Game.ShowSetCard(target);
            }
            return 0;
        }

    }
}
