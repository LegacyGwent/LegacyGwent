using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22006")]//呢喃婆：贡品
    public class WhispessTribute : CardEffect
    {//从牌组打出1张铜色/银色“有机”牌。
        public WhispessTribute(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.HasAllCategorie(Categorie.Organic) && x.IsAnyGroup(Group.Copper, Group.Silver)).ToList();
            var select = await Game.GetSelectMenuCards(PlayerIndex, cards);
            if (!select.TrySingle(out var target))
            {
                return 0;
            }
            await target.MoveToCardStayFirst();
            return 1;
        }
    }
}