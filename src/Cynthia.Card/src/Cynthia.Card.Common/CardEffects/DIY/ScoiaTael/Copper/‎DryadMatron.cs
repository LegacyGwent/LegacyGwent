using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;


namespace Cynthia.Card
{
    [CardEffectId("70122")]//树精族母 DryadMatron
    public class DryadMatron : CardEffect
    {//使同排其他树人单位获得1点强化，每强化1个树人，使手牌中随机非间谍单位牌获得1点增益。
        public DryadMatron(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var dryads = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Where(x => x != Card && x.HasAnyCategorie(Categorie.Dryad))
                .ToList();

            foreach (var dryad in dryads)
            {
                await dryad.Effect.Strengthen(1, Card);
                var handTargets = Game.PlayersHandCard[PlayerIndex]
                    .Where(x => x.CardInfo().CardType == CardType.Unit &&
                        x.CardInfo().CardUseInfo == CardUseInfo.MyRow)
                    .ToList();
                if (handTargets.TryMessOne(out var handTarget, RNG))
                {
                    await handTarget.Effect.Boost(1, Card);
                }
            }
            return 0;
        }
        
       
    }
}
