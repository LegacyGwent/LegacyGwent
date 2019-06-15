using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12024")]//叶奈法
    public class Yennefer : CardEffect
    {//择一：生成“独角兽”：使除自身外所有单位获得2点增益；或生成“梦魇独角兽”：对除自身外所有单位造成2点伤害。
        public Yennefer(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return await Game.CreateAndMoveStay(Card.PlayerIndex, new[] { CardId.Unicorn, CardId.Chironex }, 1);
        }
    }
}