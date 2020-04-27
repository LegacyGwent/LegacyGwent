using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.IO;

namespace Cynthia.Card.AI
{
    public abstract class RandomAutoAIPlayer : AIPlayer
    {
        public RandomAutoAIPlayer() : base()
        {
        }

        public override void GetMulliganInfo(Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.MulliganInfo, -1));//-1表示不进行调度
        }

        public override void GetPlayerDrag(Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.RoundOperate, GetRandomPlay()));
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
                var result = new List<int>()
                    {
                        info.SelectList.Indexed().First(x=>x.Value.Name=="0").Key
                    };
                send(
                    Operation.Create(UserOperationType.SelectMenuCardsInfo,
                    result
                ));
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
    }
}
