using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12012")]//阿瓦拉克
    public class AvallacH : CardEffect
    {//休战：双方各抽2张牌。
        public AvallacH(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (Game.IsPlayersPass[Game.AnotherPlayer(Card.PlayerIndex)]) return 0;
            await Game.DrawCard(2, 2);
            return 0;
        }
    }
}