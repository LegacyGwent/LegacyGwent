using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card.AI
{
    public class GeraltNovaAI : RandomAutoAIPlayer
    {
        public GeraltNovaAI() : base()
        {
        }

        public override void GetPlayerDrag(Action<Operation<UserOperationType>> send)
        {
            var pass = false;

            //如果不是必须取胜的场合
            if (!Data.IsMustWin)
            {
                //对方不pass的话,点数领先40就pass
                if (!Data.IsEnemyPlayerPass)
                {
                    pass = Data.MyPoint - Data.EnemyPoint > 20;
                }
                //对方pass的话,点数差在40以内就追
                else if (Data.EnemyPoint - Data.MyPoint < 30)
                {
                    pass = Data.MyPoint > Data.EnemyPoint;
                }
            }
            //如果这局必须赢,在对方已经Pass的情况,且点数领先的话
            else if (Data.IsMustWin &&
                Data.IsEnemyPlayerPass &&//三寒鸦不需要判断 !Data.EnemyPlace.SelectMany(x => x).Any(x => x.CardId == CardId.Villentretenmerth && x.IsCountdown == true) &&
                Data.MyPoint > Data.EnemyPoint)
            {
                //有盖卡点数领先40选择pass
                if (Data.EnemyPlace.SelectMany(x => x).Any(x => x.IsCardBack))
                {
                    pass = Data.MyPoint - Data.EnemyPoint > 20;
                }
                //没有盖卡直接pass
                else
                {
                    pass = true;
                }
            }

            if (pass)
            {
                send(Operation.Create(UserOperationType.RoundOperate, GetPassPlay()));
            }
            else
            {
                send(Operation.Create(UserOperationType.RoundOperate, GetRandomPlay()));
            }
        }

        public override void SetDeckAndName()
        {
            PlayerName = "木桩子一号";
            Deck = new DeckModel()
            {
                Name = "新星杰洛特",
                Leader = CardId.CiriNova,
                Deck = (CardId.CiriNova).Plural(20).Concat(CardId.GeraltOfRivia.Plural(20)).ToList()
            };
        }
    }
}