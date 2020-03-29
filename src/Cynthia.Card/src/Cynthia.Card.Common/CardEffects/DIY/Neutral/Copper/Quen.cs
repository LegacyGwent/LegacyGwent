using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70001")]//昆恩护盾
    public class Quen : CardEffect
    {//给予手牌中一个铜色单位及其牌组中的同名卡一层护盾，阻挡一次削弱/伤害/重置效果。
        public Quen(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var unitHandCard = Game.PlayersHandCard[PlayerIndex].Where(x => x.Status.Type == CardType.Unit).ToList();
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