using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44009")]//科德温骑士
    public class KaedweniKnight : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardToCemetery>
    {//若从牌组打出，则获得5点增益。 2点护甲。
        public KaedweniKnight(GameCard card) : base(card) { }
        //妥协写法
        private bool _boostflag = true;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(3, Card);
            if (_boostflag)
            {
                _boostflag = false;
                await Card.Effect.Boost(5, Card);
            }
            return 0;
        }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            _boostflag = Card.Status.CardRow.IsInDeck();

            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            if (@event.Target == Card)
            {
                _boostflag = false;
            }
            await Task.CompletedTask;

        }
    }
}