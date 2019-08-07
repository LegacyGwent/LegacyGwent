using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64012")]//图尔赛克家族好斗分子
    public class TuirseachSkirmisher : CardEffect, IHandlesEvent<AfterCardResurrect>
    {//被复活后获得3点强化。
        public TuirseachSkirmisher(GameCard card) : base(card) { }

        // private bool _resurrectflag = false;
        // public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        // {
        //     if (_resurrectflag)
        //     {
        //         _resurrectflag = false;
        //         await Card.Effect.Strengthen(3, Card);
        //         return 0;

        //     }
        //     return 0;
        // }
        public async Task HandleEvent(AfterCardResurrect @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            await Card.Effect.Strengthen(3, Card);
        }
    }
}