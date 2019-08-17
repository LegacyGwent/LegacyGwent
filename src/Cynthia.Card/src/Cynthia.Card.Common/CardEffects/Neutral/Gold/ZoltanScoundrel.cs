using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;


namespace Cynthia.Card
{
    [CardEffectId("12015")]//卓尔坦：流氓
    public class ZoltanScoundrel : CardEffect
    {//择一：生成“话篓子：伙伴”：使2个相邻单位获得2点增益；或生成“话篓子：捣蛋鬼”：对2个相邻单位造成2点伤害。
        public ZoltanScoundrel(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //根据https://www.youtube.com/watch?v=BEWI3pCjzl8 1：16直接选怪生成
            var list = new List<string>() { CardId.FieldMarshalDudaAgitator, CardId.FieldMarshalDudaCompanion };
            var count = (await Game.CreateAndMoveStay(PlayerIndex, list.ToArray()));
            if (count == 0)
            {
                return 0;
            }
            return 1;
        }
    }
}