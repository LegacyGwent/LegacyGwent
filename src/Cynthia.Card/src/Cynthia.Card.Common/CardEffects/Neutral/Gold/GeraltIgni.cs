using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("12002")]//杰洛特:伊格尼
    public class GeraltIgni : CardEffect
    {
        public GeraltIgni(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var place = Game.PlayersPlace[Game.AnotherPlayer(Card.PlayerIndex)];
            var rowlist = new List<RowPosition>();
            if (place[0].IgnoreConcealAndDead().SelectToHealth().Sum(x => x.health) >= 25)
                rowlist.Add(RowPosition.EnemyRow1);
            if (place[1].IgnoreConcealAndDead().SelectToHealth().Sum(x => x.health) >= 25)
                rowlist.Add(RowPosition.EnemyRow2);
            if (place[2].IgnoreConcealAndDead().SelectToHealth().Sum(x => x.health) >= 25)
                rowlist.Add(RowPosition.EnemyRow3);
            var row = await Game.GetSelectRow(Card.PlayerIndex, Card, rowlist);
            if (!row.IsOnPlace()) return 0;
            var list = Game.RowToList(Card.PlayerIndex, row).IgnoreConcealAndDead().WhereAllHighest();
            foreach (var card in list.ToList())
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;
        }
    }
}