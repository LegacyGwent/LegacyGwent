using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13015")]//欧吉尔德·伊佛瑞克
    public class OlgierdVonEverec : CardEffect
    {//遗愿：复活至原位。
        public OlgierdVonEverec(IGwentServerGame game, GameCard card) : base(game, card) { }
        private bool _resurrectFlag = false;
        private CardLocation _resurrectTarget = null;
        public override async Task OnCardDeath(GameCard taget, CardLocation soure)
        {
            if (taget != Card) return;
            _resurrectTarget = soure;
            _resurrectFlag = true;
            await Task.CompletedTask;
            return;
        }
        public override async Task OnTurnOver(int playerIndex)
        {
            if (playerIndex == Card.PlayerIndex && _resurrectFlag == true)
            {
                await Card.Effect.Resurrect(_resurrectTarget, Card);
                _resurrectFlag = false;
            }
        }
    }
}