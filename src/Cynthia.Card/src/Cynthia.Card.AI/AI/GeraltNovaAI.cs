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
                Deck = (CardId.CiriNova).Plural(5).Concat(CardId.GeraltOfRivia.Plural(25)).ToList()
            };
        }
        // public override void SetDeckAndName()
        // {
        //     PlayerName = "店店1";
        //     Deck = new DeckModel()
        //     {
        //         Name = "店店测试",
        //         Leader = CardId.BrouverHoog,//布罗瓦尔霍格

        //         Deck = (CardId.GeraltOfRivia).Plural(1)//利维亚的杰洛特
        //         .Concat(CardId.AglaS.Plural(1))//艾格莱丝
        //         .Concat(CardId.ShupeSDayOff.Plural(1))//店店的大冒险
        //         .Concat(CardId.Muzzle.Plural(1))//嘴套
        //         .Concat(CardId.Roach.Plural(1))//萝卜
        //         .Concat(CardId.IdaEmeanAepSivney.Plural(1))//艾达艾敏
        //         .Concat(CardId.Vaedermakar.Plural(1))//德鲁伊控天者
        //         .Concat(CardId.IbhearHattori.Plural(1))//哈托利
        //         .Concat(CardId.BarclayEls.Plural(1))//巴克莱艾尔斯
        //         .Concat(CardId.Decoy.Plural(1))//诱饵
        //         .Concat(CardId.VriheddNeophyte.Plural(1))//维里赫德旅新兵
        //         .Concat(CardId.DwarvenMercenary.Plural(1))//矮人佣兵
        //         .Concat(CardId.DolBlathannaArcher.Plural(1))//多尔布雷坦纳弓箭手
        //         .Concat(CardId.HawkerSupport.Plural(1))//私枭后援者
        //         .Concat(CardId.DolBlathannaBomber.Plural(1))//多尔布雷坦纳爆破手
        //         .Concat(CardId.DwarvenSkirmisher.Plural(1))//矮人好斗分子
        //         .Concat(CardId.HalfElfHunter.Plural(1))//半精灵猎人
        //         .Concat(CardId.Pyrotechnician.Plural(1))//烟火技师
        //         .Concat(CardId.MahakamGuard.Plural(1))//玛哈坎守卫
        //         .Concat(CardId.Panther.Plural(1))//黑豹
        //         .Concat(CardId.AvallacHTheSage.Plural(1))//贤者
        //         .Concat(CardId.ElvenMercenary.Plural(1))//精灵佣兵
        //         .Concat(CardId.DwarvenAgitator.Plural(1))//矮人煽动分子
        //         .Concat(CardId.AlzurSThunder.Plural(1))//落雷术
        //         .Concat(CardId.Reconnaissance.Plural(1))//侦察
        //         .ToList()
        //     };
        //}
    }
}