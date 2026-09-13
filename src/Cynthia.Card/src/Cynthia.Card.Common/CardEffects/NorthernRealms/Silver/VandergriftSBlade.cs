using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43021")]//范德格里夫特之剑
    public class VandergriftSBlade : CardEffect, IHandlesEvent<AfterRoundOver>
    {//造成10点伤害，摧毁铜色/银色诅咒生物，放逐所摧毁的单位。小局结束时从墓场返回牌组，伤害提升3点。
        private int _damage = 10;

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
            await target.Effect.Damage(_damage, Card);
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

        public async Task HandleEvent(AfterRoundOver @event)
        {
            if (!Card.Status.CardRow.IsInCemetery()) return;

            _damage += 3;
            await Game.ShowCardMove(new CardLocation(
                RowPosition.MyDeck, RNG.Next(Game.PlayersDeck[PlayerIndex].Count + 1)), Card);
        }
    }
}
