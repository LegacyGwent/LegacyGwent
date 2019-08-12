using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43017")]//萨宾娜·葛丽维希格
    public class SabrinaGlevissig : CardEffect, IHandlesEvent<AfterCardDeath>
    {//间谍。 遗愿：将该排最弱单位的战力应用于全排单位。
        public SabrinaGlevissig(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            var rowlist = Game.RowToList(Card.PlayerIndex, Card.GetLocation().RowPosition).Where(x => x.GetLocation().RowPosition.IsOnPlace() && x != Card).ToList();
            if (rowlist.Count() <= 1)
            {
                return;
            }
            int minhealth = rowlist.WhereAllLowest().First().CardPoint();
            foreach (var card in rowlist)
            {
                int offnum = card.CardPoint() - minhealth;
                if (offnum > 0)
                {
                    await card.Effect.Damage(offnum, Card);
                }
            }
            return;
        }

    }
}