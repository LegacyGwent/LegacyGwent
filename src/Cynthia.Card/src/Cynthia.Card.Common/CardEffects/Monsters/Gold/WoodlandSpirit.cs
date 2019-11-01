using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22008")]//林妖
    public class WoodlandSpirit : CardEffect
    {//在近战排生成3只“狼”，并在对方同排降下“蔽日浓雾”。
        public WoodlandSpirit(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 3; i++)
            {
                await Game.CreateCardAtEnd(CardId.Wolf, PlayerIndex, RowPosition.MyRow1);
            }
            await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()].SetStatus<ImpenetrableFogStatus>();
            return 0;
        }
    }
}