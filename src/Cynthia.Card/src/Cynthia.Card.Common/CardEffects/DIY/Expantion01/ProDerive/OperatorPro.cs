using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130160")]//操作者：晋升
    public class OperatorPro : CardEffect
    {//力竭。休战：为双方各添加1张己方手牌1张非“力竭”铜色/银色单位牌的原始同名牌。
        public OperatorPro(GameCard card) : base(card) { }

        public bool IsUse { get; set; } = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            if (Game.IsPlayersPass[AnotherPlayer])
            {
                return 0;
            }
            var cards = Game.PlayersHandCard[PlayerIndex].Where(x => 
            (x.Is(Group.Copper, CardType.Unit)||x.Is(Group.Silver, CardType.Unit)) &&
            x.CardInfo().CardId != CardId.Operator && x.CardInfo().CardId != CardId.Udalryk &&
            x.CardInfo().CardId != CardId.Cantarella && x.CardInfo().CardId != CardId.Ocvist &&
            x.CardInfo().CardId != CardId.Yaevinn && x.CardInfo().CardId != CardId.Thaler &&
            x.CardInfo().CardId != CardId.Frightener && x.CardInfo().CardId != CardId.Highwaymen &&
            x.CardInfo().CardId != CardId.StraysofSpalla).ToList();
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