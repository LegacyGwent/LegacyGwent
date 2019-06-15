using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("15004")]//梦魇独角兽
    public class Chironex : CardEffect
    {//对所有其他单位造成2点伤害。
        public Chironex(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.Status.CardRow.IsOnPlace() && x != Card).ToList();
            foreach (var card in cards)
            {
                await card.Effect.Damage(2, Card, BulletType.RedLight);
            }
            return 0;
        }
    }
}