using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12039")]//乌马的诅咒
    public class UmaSCurese : CardEffect
    {//根据场上最高战力单位的所在排及战力奇偶，生成1个对应奇偶战力的己方起始牌组之外的非领袖金色单位。
        public UmaSCurese(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var orderedRows = new[]
            {
                RowPosition.EnemyRow3,
                RowPosition.EnemyRow2,
                RowPosition.EnemyRow1,
                RowPosition.MyRow1,
                RowPosition.MyRow2,
                RowPosition.MyRow3
            };
            var orderedCards = orderedRows
                .SelectMany(row => Game.RowToList(PlayerIndex, row)
                    .IgnoreConcealAndDead()
                    .Select(card => new { Card = card, Row = row }))
                .ToList();
            if (orderedCards.Count == 0) return 0;

            var highestPower = orderedCards.Max(item => item.Card.CardPoint());
            var firstHighest = orderedCards.First(item => item.Card.CardPoint() == highestPower);
            var faction = Faction.Neutral;
            switch (firstHighest.Row)
            {
                case RowPosition.EnemyRow3:
                    faction = Faction.Skellige;
                    break;
                case RowPosition.EnemyRow2:
                    faction = Faction.ScoiaTael;
                    break;
                case RowPosition.EnemyRow1:
                    faction = Faction.NorthernRealms;
                    break;
                case RowPosition.MyRow1:
                    faction = Faction.Nilfgaard;
                    break;
                case RowPosition.MyRow2:
                    faction = Faction.Monsters;
                    break;
            }

            var parity = Math.Abs(highestPower % 2);
            var candidates = GwentMap.GetGenerateCardsId(
                card => card.Is(Group.Gold, CardType.Unit) &&
                    card.Faction == faction &&
                    Math.Abs(card.Strength % 2) == parity,
                Card.GetMyBaseDeck().Select(card => card.CardId));
            return await Card.CreateAndMoveStay(candidates.ToList());
        }
    }
}
