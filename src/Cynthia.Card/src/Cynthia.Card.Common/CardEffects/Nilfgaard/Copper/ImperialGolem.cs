using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34025")]//帝国魔像
    public class ImperialGolem : CardEffect, IHandlesEvent<AfterCardReveal>
    {//每当己方揭示1张对方手牌，便从牌组召唤1张同名牌。
        public ImperialGolem(GameCard card) : base(card) { }
        private static GameCard temp;

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (Card.Status.CardRow != RowPosition.MyDeck || temp == @event.Target || (@event.Target.PlayerIndex == Card.PlayerIndex) || @event.Source == null || @event.Source.PlayerIndex != Card.PlayerIndex && @event.Source.Status.CardId != "70174") return;
            temp = @event.Target;
            await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), @event.Target);//(Game.GetRandomCanPlayLocation(Card.PlayerIndex));
        }
    }
}