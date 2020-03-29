using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70001")]//昆恩护盾
    public class Quen : CardEffect
    {//选择手牌中的一个铜色/银色单位，给予其和其在手牌和牌组中的同名卡2点增益和护盾。护盾可以阻挡一次伤害效果。已经有护盾的不能被选中。
        public Quen(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var unitHandCard = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.Status.Type == CardType.Unit &&
                    x.IsAnyGroup(Group.Copper, Group.Silver) &&
                    x.Status.IsShield == false).ToList();
            if (unitHandCard.Count() == 0) { return 0; }

            var targetcards = await Game.GetSelectMenuCards(PlayerIndex, unitHandCard, isCanOver: false);
            if (!targetcards.TrySingle(out var target)) { return 0; }

            var deckCardList = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardId == target.CardInfo().CardId);
            var handCardList = Game.PlayersHandCard[Card.PlayerIndex].Where(x => x.CardInfo().CardId == target.CardInfo().CardId);

            foreach (var card in handCardList)
            {
                card.Status.IsShield = true;
            }

            foreach (var card in deckCardList)
            {
                card.Status.IsShield = true;
            }
            return 0;
        }
    }
}