using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24029")]//食尸鬼
    public class Ghoul : CardEffect
    {//吞噬墓场中1个铜色/银色单位，获得等同于其战力的增益。
        public Ghoul(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersCemetery[PlayerIndex].FilterCards(type: CardType.Unit, filter: x => x.IsAnyGroup(Group.Copper, Group.Silver)).ToList());
            if (!targets.TrySingle(out var target))
            {
                return 0;
            }
            await Consume(target);
            return 0;
        }
    }
}