using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64005")]//狂战士掠夺者
    public class BerserkerMarauder : CardEffect
    {//场上每有1个受伤、或为“诅咒生物”的友军单位，便获得1点增益。
        public BerserkerMarauder(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于,受伤的友军诅咒单位只会给本卡提供一点增益


            // //友方诅咒非受伤单位
            // var listHurtnotCursed = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => !x.HasAllCategorie(Categorie.Cursed) && x != Card && x.Status.HealthStatus < 0);
            // //友方受伤非诅咒单位
            // var listCursednotHurt = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Cursed) && x != Card && x.Status.HealthStatus >= 0);
            // //友方受伤诅咒单位			
            // var listCursedandHurt = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Cursed) && x != Card && x.Status.HealthStatus < 0);
            // int Boostnum = listHurtnotCursed.Count() + listCursednotHurt.Count() + listCursedandHurt.Count();
            // if (Boostnum == 0)
            // {
            //     return 0;
            // }
            // await Card.Effect.Boost(Boostnum, Card);
            // return 0;

            var listHurt = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x != Card && x.Status.HealthStatus < 0).ToList();
            var listCursed = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Cursed) && x != Card).ToList();
            //无交并
            var result = listHurt.Union(listCursed);

            int Boostnum = result.Count();
            if (Boostnum == 0)
            {
                return 0;
            }
            await Card.Effect.Boost(Boostnum, Card);
            return 0;

        }
    }
}