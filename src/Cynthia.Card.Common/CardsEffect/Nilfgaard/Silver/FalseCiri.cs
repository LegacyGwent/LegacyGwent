using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("33002")]//冒牌希里
    public class FalseCiri : CardEffect
    {
        public FalseCiri(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task OnTurnStart(int playerIndex)//回合开始时触发的事件
        {
            //我方回合开始时,如果没有被锁定,并且是间谍的话
            if (Card.PlayerIndex == playerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Boost(1);
        }
        public override async Task OnPlayerPass(int playerIndex)//有玩家pass触发的事件
        {
            if (Card.PlayerIndex == playerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Charm();
        }
        public override async Task OnCardDeath(GameCard taget,CardLocation source)//死亡时触发的事件
        {
            if(Card.Status.IsLock)return;
            if (taget == Card)
            {
                var row = Game.RowToList(taget.PlayerIndex,source.RowPosition);
                var cards = row.WhereAllLowest().ToList();
                for(var i = 0; i<cards.Count;i++)
                {
                    await cards[i].Effect.ToCemetery(CardBreakEffectType.Scorch);
                }
            }
        }
    }
}