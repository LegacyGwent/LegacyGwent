using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12012")]//阿瓦拉克
    public class AvallacH : CardEffect
    {//休战：双方各抽2张牌。
        public AvallacH(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            if (Game.IsPlayersPass[Game.AnotherPlayer(Card.PlayerIndex)]) return 0;
            await Game.DrawCard(2, 2);
            return 0;
        }
    }
}