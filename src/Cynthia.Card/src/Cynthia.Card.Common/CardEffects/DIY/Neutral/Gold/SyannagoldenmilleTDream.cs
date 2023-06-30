using System.Linq;
using System.Treading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
     [CardEffectId("70082")]//席安娜:黄粱一梦
      public class SyannagoldenmilleTDream : CardEffect
      {//使场上战力最高的牌获得“佚亡”效果。若其已被增益，则使其直接被放逐。
      public SyannagoldenmilleTDream(GameCard card) : base(card) { }
      public override async Task<int> CardUseEffect()
          {
             var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).where(x =>x.Status.CardRow.IsOnPlace()).whereAllHighest().ToList();
             foreach (var card in cards)
             {
               if (card.status.Healthstatus > 0 )
                {
                  card.status.IsDoomed = true;
                  await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                }
               else
                {
                  card.status.IsDoomed = true;
                }
             }
            return 0;
           }
       }
}
