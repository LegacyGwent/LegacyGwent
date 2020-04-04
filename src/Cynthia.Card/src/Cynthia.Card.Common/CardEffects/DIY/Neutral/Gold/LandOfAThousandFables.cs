using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70014")]//童话国度：公正女神
    public class LandOfAThousandFables : CardEffect, IHandlesEvent<AfterPlayerPass>, IHandlesEvent<BeforeRoundStart>
    {//游戏开始时，将本卡置入墓地。\n双方都放弃跟牌后，给先手方增加自身战力的点数，然后放逐自身。\n免疫。无法被召唤、复活、强化、削弱、增益、伤害、魅惑、变形。
        public LandOfAThousandFables(GameCard card) : base(card) { }
        private int passedCount = 0;
        public async Task HandleEvent(BeforeRoundStart @event)
        {
            // 游戏开始的时候，在场上让玩家看清楚战力，然后送入墓地
            Card.Status.IsImmue = true;
            await Game.ClientDelay(200);
            // 延迟是为了先在场上让玩家看清楚战力，然后移到墓地里。

            // 这里override ToCemetery比较麻烦，选择了加入两个新参数，取消banish和sendevent
            await Card.Effect.ToCemetery(isNeedBanish: false, isNeedSentEvent: false);
            return;
        }
        public async Task HandleEvent(AfterPlayerPass @event)
        {
            // 如果双方都pass，从墓地移动到场上（不算复活），如果场地满了，给场上任意一个单位加战力（不算增益）
            // 考虑到“场地满了”，本身触发的几率就比较小（只有铺场吞有这个潜力）
            // 并且此时游戏本身也会出bug，（比如吞噬鸟蛋），所以等游戏本身的bug修复了，再考虑本卡“如果场地满了”的效果的修订
            passedCount += 1;
            if (passedCount == 2)
            {
                await Game.Debug("童话国度复活");
                //如果场地没满，转移到场地上，不属于复活。
                if (Game.GetRandomRow(PlayerIndex, out var rowIndex))
                {
                    var location = new CardLocation(rowIndex.Value, 0);
                    var rowCards = Game.RowToList(Card.PlayerIndex, location.RowPosition);
                    if (location.CardIndex > rowCards.Count)
                    {
                        location.CardIndex = rowCards.Count;
                    }
                    await Game.ShowCardMove(location, Card, true);
                    if (location.RowPosition.IsOnPlace())
                    {
                        await Game.ShowCardOn(Card);
                        await Game.ShowCardDown(Card);
                        await Game.SetPointInfo();
                    }
                    // 清理战场时会被放逐
                    Card.Status.IsDoomed = true;
                    await Game.ClientDelay(800);
                    return;
                }
                else
                {
                    //否则，增加场上随机1个单位的战力，不属于增益。
                    await Game.Debug("BOOST");
                    var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.PlayerIndex == Card.PlayerIndex && x.Status.CardRow.IsOnPlace() && x != Card).Mess(Game.RNG).ToList();
                    if (cards.Count() == 0) { return; }
                    var targetCard = cards.First();

                    int num = Card.Status.Strength;
                    await Game.ShowCardNumberChange(targetCard, num, NumberType.Normal);
                    await Game.ClientDelay(50);
                    targetCard.Status.HealthStatus += num;
                    await Game.ShowSetCard(targetCard);
                    await Game.SetPointInfo();

                    // 放逐自身。因为自身没有在墓地里，直接放逐不影响场上比分。
                    await Card.Effect.Banish();
                    await Game.Debug("FINISH");
                    // 延迟方便玩家看清楚
                    await Game.ClientDelay(800);
                }
            }
            return;
        }

        // 下面override一些方法，使得本卡无法被召唤、复活、强化、削弱、增益、伤害、魅惑、变形

        public override async Task Summon(CardLocation location, GameCard source)//召唤
        {
            // 无法被召唤
            await Task.CompletedTask;
            return;
        }

        public override async Task Resurrect(CardLocation location, GameCard source)//复活
        {
            // 无法被复活
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

        public override async Task Transform(string cardId, GameCard source, System.Action<GameCard> setting = null, bool isForce = false)//变为
        {
            // 无法被变形
            await Task.CompletedTask;
            return;
        }
    }
}