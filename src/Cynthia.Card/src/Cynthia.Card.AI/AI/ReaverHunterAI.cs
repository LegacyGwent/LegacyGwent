using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card.AI
{
    public class ReaverHunterAI : AIPlayer
    {
        public ReaverHunterAI() : base()
        {
        }

        public override void GetMulliganInfo(Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.MulliganInfo, -1));//-1表示不进行调度
        }

        public override void GetPlayerDrag(Action<Operation<UserOperationType>> send)
        {
            var pass = false;

            //如果不是必须取胜的场合,并且对方不pass的话,点数领先40就pass
            if (!Data.IsMustWin)
            {
                if (!Data.IsEnemyPlayerPass)
                {
                    pass = Data.MyPoint - Data.EnemyPoint > 30;
                }
                else
                {
                    pass = Data.MyPoint > Data.EnemyPoint;
                }
            }
            //如果这局必须赢, 在对方已经Pass的情况,如果没有三寒鸦并且点数领先的话
            else if (Data.IsMustWin &&
                Data.IsEnemyPlayerPass &&
                !Data.EnemyPlace.SelectMany(x => x).Any(x => x.CardId == CardId.Villentretenmerth && x.IsCountdown == true) &&
                Data.MyPoint > Data.EnemyPoint)
            {
                //有盖卡点数领先40选择pass
                if (Data.EnemyPlace.SelectMany(x => x).Any(x => x.IsCardBack))
                {
                    pass = Data.MyPoint - Data.EnemyPoint > 40;
                }
                //没有盖卡直接pass
                else
                {
                    pass = true;
                }
            }

            //否则随机出牌到随机位置

            if (pass)
            {
                send(Operation.Create(UserOperationType.RoundOperate, GetPassPlay()));
            }
            else
            {
                send(Operation.Create(UserOperationType.RoundOperate, GetRandomPlay()));
            }
        }

        public override void PlayCard(CardLocation location, Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.PlayCardInfo, CardCanPlay(Data.GetCard(location).CardId.CardInfo().CardUseInfo).Mess().First()));
        }

        public override void SelectMenuCards(MenuSelectCardInfo info, Action<Operation<UserOperationType>> send)
        {
            //先后手固定选0
            if (info.Title == "请选择你认为后手价值的点数")
            {
                send(Operation.Create(UserOperationType.SelectMenuCardsInfo, new List<int>() { info.SelectList.IndexOf(x => x.Strength == 0) }));
            }
            else
            {
                send(Operation.Create(UserOperationType.SelectMenuCardsInfo, 0.To(info.SelectList.Count - 1).Mess().Take(info.SelectCount).ToList()));
            }
        }

        public override void SelectPlaceCards(PlaceSelectCardsInfo info, Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.SelectPlaceCardsInfo, info.CanSelect.CardsPartToLocation().Mess().Take(info.SelectCount).ToList()));
        }

        public override void SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.SelectRowInfo, rowPart.Mess().First()));
        }

        public override void SetDeckAndName()
        {
            PlayerName = "掠夺者猎人团";
            Deck = new DeckModel()
            {
                Name = "团结的力量",
                Leader = CardId.ReaverHunter,
                Deck = (CardId.ReaverHunter).Plural(40).ToList()
            };
        }
    }
}