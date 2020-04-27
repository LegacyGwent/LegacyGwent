using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70018")]//掠夺者猎人噩梦铠甲
    public class ReaverHunterAITools1 : CardEffect
    {//游戏开始时,将这张卡置入墓地。若在墓地,己方卡组的单位无法从卡组移动至墓地,免疫决斗伤害,且打出时候获得1-2点伤害或1-2点增益。

        public ReaverHunterAITools1(GameCard card) : base(card) { }


    }
}
