using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13007")]//斯崔葛布
    public class Stregobor : CardEffect
    {//力竭。休战：双方各抽1张单位牌，将其战力设为1。
        public Stregobor(GameCard card) : base(card) { }
        public bool IsUse { get; set; } = false;
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            if (Game.IsPlayersPass[Game.AnotherPlayer(Card.PlayerIndex)]) return 0;
            var cards = await Game.DrawCard(1, 1, x => x.CardInfo().CardType == CardType.Unit);
            foreach (var item in cards.Item1.Concat(cards.Item2))
            {
                await item.Effect.Damage(item.ToHealth().health - 1, item, BulletType.Lightnint);
            }
            return 0;
        }
    }
}