using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53016")] //保利·达尔伯格
    public class PaulieDahlberg : CardEffect
    {
        //复活一个铜色非“辅助”矮人单位。
        public PaulieDahlberg(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersCemetery[PlayerIndex].Where(CooperNoSupportDwarf);

            if (!(await Game.GetSelectMenuCards(PlayerIndex, cards.ToList())).TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Resurrect(new CardLocation(RowPosition.MyStay, 0), Card);
            return 1;
        }

        private bool CooperNoSupportDwarf(GameCard card)
        {
            return card.Status.Categories.Contains(Categorie.Dwarf) && card.Status.Group == Group.Copper &&
                   !card.Status.Categories.Contains(Categorie.Support);
        }
    }
}