using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62002")]//哈尔玛·奎特
    public class Hjalmar : CardEffect
    {//在对方同排生成“乌德维克之主”。
        public Hjalmar(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(CardId.LordOfUndvik, AnotherPlayer, Card.GetLocation());
            return 0;
        }
    }
}