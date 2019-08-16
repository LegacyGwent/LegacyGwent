using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42012")]//弗农·罗契
    public class VernonRoche : CardEffect, IHandlesEvent<AfterTurnStart>
    {//对1个敌军单位造成7点伤害。 对局开始时，将1个“蓝衣铁卫突击队”加入牌组。
        public VernonRoche(GameCard card) : base(card) { }
        private bool isuse = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(7, Card);
            return 0;
        }
        //以下是妥协的写法：判断对局开始
        //回合开始时，如果roche在卡组中且flag为false，把蓝衣铁卫突击队创造进卡组。然后使flag为true。创造出来的罗契不可能回到手牌 从而不可能回到卡组
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (!isuse && Card.GetLocation().RowPosition == RowPosition.MyDeck)
            {
                isuse = true;
                await Game.CreateCard(CardId.BlueStripesCommando, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            }
            return;
        }

    }
}