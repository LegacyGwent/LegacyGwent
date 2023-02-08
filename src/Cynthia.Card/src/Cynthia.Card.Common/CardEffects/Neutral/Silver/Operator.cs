using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13016")]//操作者
    public class Operator : CardEffect
    {//力竭。 休战：为双方各添加1张己方手牌1张铜色单位牌的原始同名牌。
        public Operator(GameCard card) : base(card) { }

        public bool IsUse { get; set; } = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            if (Game.IsPlayersPass[AnotherPlayer])
            {
                return 0;
            }
            var cards = Game.PlayersHandCard[PlayerIndex].Where(x => x.Is(Group.Copper, CardType.Unit)).ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, cards)).TrySingle(out var target))
            {
                return 0;
            }
            var id = target.Status.CardId;
            await Game.CreateCardAtEnd(id, PlayerIndex, RowPosition.MyHand);
            await Game.CreateCardAtEnd(id, AnotherPlayer, RowPosition.MyHand);
            return 0;
        }
    }
}