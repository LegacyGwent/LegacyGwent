using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70193")]//化妆舞会 Masquerade
    public class Masquerade : CardEffect
    {
        public Masquerade(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var targets = await Game.GetSelectPlaceCards(
                Card,
                filter: target => target.Status.Type == CardType.Unit &&
                                  target.Status.Group != Group.Leader,
                selectMode: SelectModeType.AllRow);
            if (!targets.TrySingle(out var target))
            {
                return 0;
            }

            var options = new List<(Group Group, string Name, string Info)>
            {
                (Group.Gold, "变为金色", "Masquerade_ChangeGold"),
                (Group.Silver, "变为银色", "Masquerade_ChangeSilver"),
                (Group.Copper, "变为铜色", "Masquerade_ChangeCopper")
            }
            .Where(option => option.Group != target.Status.Group)
            .ToList();
            var menu = options
                .Select(option => new CardStatus(Card.Status.CardId)
                {
                    Name = option.Name,
                    Info = option.Info
                })
                .ToList();
            var selected = await Game.GetSelectMenuCards(
                PlayerIndex,
                menu,
                isCanOver: true,
                title: "选择品质");
            if (!selected.TrySingle(out var selectedIndex))
            {
                return 0;
            }

            target.Status.Group = options[selectedIndex].Group;
            await Game.ShowSetCard(target);
            await Game.SetPointInfo();
            return 0;
        }
    }
}
