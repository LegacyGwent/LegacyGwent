using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12029")]//阿瓦拉克：贤者
    public class AvallacHTheSage : CardEffect
    {//随机生成1张对方起始牌组中金色/银色单位牌的原始同名牌。
        public AvallacHTheSage(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var ids = Game.PlayerBaseDeck[AnotherPlayer].Deck
                .Where(x => x.Is(filter: x => x.IsAnyGroup(Group.Gold, Group.Silver) && x.CardId != Card.Status.CardId, type: CardType.Unit))
                .Select(x => x.CardId);
            if (!ids.TryMessOne(out var createId, Game.RNG))
            {
                return 0;
            }
            await Game.CreateToStayFirst(createId, PlayerIndex);
            return 1;
        }
    }
}