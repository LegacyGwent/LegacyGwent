using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("80001")]//奥贝伦王
    public class AuberonKing : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<BeforeRoundStart>, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterUnitDown>
    {//
        public AuberonKing(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            Card.Status.IsImmue = true;
            await Card.Effect.Resilience(Card);
            return 0;
        }

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            if (@event.Target != Card)
            {
                return;
            }

            if (@event.DeathLocation.RowPosition.IsOnPlace())
            {
                await Card.Effect.Resurrect(@event.DeathLocation, Card);
                await Card.Effect.Resilience(Card);
                Card.Status.IsImmue = true;

            }
            await Task.CompletedTask;
        }

        public async Task HandleEvent(BeforeRoundStart @event)
        {
            if (!Game.GetRandomRow(PlayerIndex, out var rowIndex) || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            await Card.Effect.Transform(CardId.AuberonInvader, Card,isForce:true);
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var allenemylist = new List<RowPosition>() { RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3 };
            var rowlist = new List<RowPosition>();
            var targetRow = new List<RowPosition>();
            foreach (var row in allenemylist)
            {
                if (Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()].RowStatus.IsHazard() == false)
                {
                    rowlist.Add(row);
                }
                if (Game.RowToList(AnotherPlayer, row).IgnoreConcealAndDead().Count() < Game.RowMaxCount || Game.RowToList(AnotherPlayer, row).IgnoreConcealAndDead().Count() < 3 || Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()].RowStatus.IsHazard())
                {
                    targetRow.Add(row);
                }
            }
            if (rowlist.Count() == 0)
            {
                var targetCards = Game.GetPlaceCards(AnotherPlayer).Where(x => x.Status.CardRow.IsOnPlace());

                if (targetCards.Count() != 0 && targetRow.Count() != 0)
                {
                    var targetCards1 = targetCards.Mess(RNG).First();
                    var targetRow1 = targetRow.Mess(Game.RNG).First().Mirror();

                    if (targetRow1 != targetCards1.Status.CardRow)
                    {
                        await targetCards1.Effect.Move(new CardLocation(targetRow1, int.MaxValue), Card);
                    }

                }

                return;
            }
            var TargetRow = rowlist.Mess(Game.RNG).First();
            await Game.GameRowEffect[AnotherPlayer][TargetRow.Mirror().MyRowToIndex()].SetStatus<BitingFrostStatus>();

            return;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target == Card || !@event.Target.HasAllCategorie(Categorie.WildHunt) || !Card.IsAliveOnPlance() || @event.Target.PlayerIndex != PlayerIndex || (@event.IsMoveInfo.isMove && !@event.IsMoveInfo.isFromeEnemy))
            {
                return;
            }
            await @event.Target.Effect.Boost(1, Card);
        }

        // 下面override一些方法，使得本卡无法被召唤、复活、强化、削弱、增益、伤害、魅惑、变形

        public override async Task Summon(CardLocation location, GameCard source)//召唤
        {
            // 无法被召唤
            await Task.CompletedTask;
            return;
        }


        public override async Task Strengthen(int num, GameCard source)
        {
            // 无法被强化
            await Task.CompletedTask;
            return;
        }

        public override async Task Weaken(int num, GameCard source)
        {
            // 无法被削弱
            await Task.CompletedTask;
            return;
        }

        public override async Task Boost(int num, GameCard source)
        {
            // 无法被增益
            await Task.CompletedTask;
            return;
        }

        public override async Task Damage(int num, GameCard source, BulletType showType = BulletType.Arrow, bool isPenetrate = false, DamageType damageType = DamageType.Unit)
        {
            // 无法被伤害
            await Task.CompletedTask;
            return;
        }

        public override async Task Charm(GameCard source)//被魅惑
        {
            // 无法被魅惑
            await Task.CompletedTask;
            return;
        }

    }
}
