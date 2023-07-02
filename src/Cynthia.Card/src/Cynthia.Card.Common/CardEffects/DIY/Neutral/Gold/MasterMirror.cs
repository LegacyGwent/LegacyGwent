using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
     [CardEffectId("70082")]//镜子大师
     public class MasterMirror : CardEffect
     {//使所有最强的单位获得“佚亡”，然后摧毁其中具有增益的单位。
          public MasterMirror(GameCard card) : base(card) { }
          public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
          {
               var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x =>x.Status.CardRow.IsOnPlace()).WhereAllHighest().ToList();
               foreach (var card in cards)
               {
                    card.status.IsDoomed = true;
                    if (card.Status.HealthStatus > 0 )
                    {
                         await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                    }
               }
               return 0; 
          }
     }
}
