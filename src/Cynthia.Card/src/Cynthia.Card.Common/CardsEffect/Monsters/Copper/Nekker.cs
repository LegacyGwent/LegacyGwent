using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24031")]//孽鬼
    public class Nekker : CardEffect
    {//若位于手牌、牌组或己方半场：友军单位发动吞噬时获得1点增益。 遗愿：召唤1张同名牌。
        public Nekker(IGwentServerGame game, GameCard card) : base(game, card){ }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            await Task.CompletedTask;
            return 0;
        }

        //当友军单位发动吞噬时,并且自己在场位于手牌、牌组或己方半场时获得1点增益
        public override async Task OnCardConsume(GameCard master, GameCard taget)
        {
            if(master.PlayerIndex == Card.PlayerIndex && 
                (Card.Status.CardRow.IsOnPlace()||Card.Status.CardRow.IsInHand()||Card.Status.CardRow.IsInDeck()))
            {
                await Boost(1);
            }
        }

        //遗愿：召唤1张同名牌
        public override async Task OnCardDeath(GameCard taget, CardLocation soure)
        {
            //如果不是自己死亡，返回
            if(taget!=Card)
            {    
                return;
            }

            //在自己的牌库中寻找同名卡
            var targets = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.CardId = CardId.Nekker);

            //如果没有，直接返回
            if (targets.Count() == 0) 
            {
                return;
            }
            
            //放置死亡的位置到上面
            await targets.First().Effect.Play(soure);
            return;
        }
    }
}