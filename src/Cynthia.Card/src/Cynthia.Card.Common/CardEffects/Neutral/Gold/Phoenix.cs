using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12028")]//凤凰
    public class Phoenix : CardEffect
    {//复活1个铜色/银色“龙兽”单位。
        public Phoenix(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersCemetery[PlayerIndex].Where(x =>
                x.Is(filter: x => x.IsAnyGroup(Group.Copper, Group.Silver) &&
                x.HasAllCategorie(Categorie.Draconid),
                type: CardType.Unit)).ToList();

            var selectCard = await Game.GetSelectMenuCards(PlayerIndex, cards);
            if (!selectCard.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Resurrect(new CardLocation(RowPosition.MyStay, 0), Card);
            return 1;
        }
    }
}