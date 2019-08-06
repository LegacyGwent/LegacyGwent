using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62013")]//坎比
    public class Kambi : CardEffect, IHandlesEvent<AfterCardDeath>
    {//间谍。遗愿：生成“汉姆多尔”。
        public Kambi(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            //如果死的不是这样卡，或者死亡位置不是场上，什么事情都不做
            if (@event.Target != Card || !@event.DeathLocation.RowPosition.IsOnPlace())
            {
                return;
            }
            var row = @event.DeathLocation.RowPosition;
            //最左生成汉姆多尔
            await Game.CreateCard(CardId.Hemdall, Card.PlayerIndex, new CardLocation(row, 0));
        }
    }
}