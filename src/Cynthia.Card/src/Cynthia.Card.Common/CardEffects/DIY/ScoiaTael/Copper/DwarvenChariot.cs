using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70109")]//矮人战车
    public class DwarvenChariot : CardEffect,IHandlesEvent<AfterCardMove>
    {//选择2个单位，将它们移至所在半场的此排。自身移动后使所在排随机1个单位获得2点增益。
        public DwarvenChariot(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectCard = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.AllRow, filter: x => x.Status.CardRow != Card.Status.CardRow);
            foreach (var target in selectCard)
            {
                await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            }
            return 0;
        }
        private GameCard GetCurrentListRandomCard()
        {//获取本卡牌当前所在排随机一个单位。
            var list = Game.GetPlaceCards(Card.PlayerIndex);
            return !list.Any() ? null : list.Mess(Game.RNG).First();
        }
         public async Task HandleEvent(AfterCardMove @event)
         {
             var card = GetCurrentListRandomCard();
             if (@event.Target == Card)
            {
              if (card != null)
                {
                    await Card.Effect.Boost(2, Card);
                }
            }
         }
    }
}