using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42012")]//弗农·罗契
    public class VernonRoche : CardEffect, IHandlesEvent<OnGameStart>
    {//对1个敌军单位造成7点伤害。 对局开始时，将1个“蓝衣铁卫突击队”加入牌组。
        public VernonRoche(GameCard card) : base(card) { }
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
        public async Task HandleEvent(OnGameStart @event)
        {
            await Game.CreateCard(CardId.BlueStripesCommando, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            return;
        }
    }
}