using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24023")]//翼手龙
    public class Wyvern : CardEffect, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterRoundOver>
    {//对一个敌军单位造成5点伤害。若本小局中打出过其他龙兽单位，则改为造成7点伤害。
    // Deal 5 damage to an enemy. If you have played another Draconid unit this round, deal 7 damage instead.
        public Wyvern(GameCard card) : base(card) { }
        private int damage = 5;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(damage, Card);
            return 0;
        }
        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.HasAllCategorie(Categorie.Draconid) && @event.Target.PlayerIndex == Card.PlayerIndex)
            {
                damage = 7;
            }
            await Task.CompletedTask;
        }
        public async Task HandleEvent(AfterRoundOver @event)
        {
            damage = 5;
            await Task.CompletedTask;
        }
    }
}