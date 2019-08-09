using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23014")]//吸血夜魔
    public class Nekurat : CardEffect
    {//生成“月光”。
        public Nekurat(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateToStayFirst(CardId.Moonlight, Card.PlayerIndex);
            return 1;
        }
    }
}