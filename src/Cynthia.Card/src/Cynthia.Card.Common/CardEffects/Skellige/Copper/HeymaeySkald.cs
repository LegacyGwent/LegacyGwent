using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("64006")]//海玫家族诗人
    public class HeymaeySkald : CardEffect
    {//使所选“家族”的所有友军单位获得2点增益。
        public HeymaeySkald(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选一张场上的家族单位,不能是自己
            var target = await Game.GetSelectPlaceCards(Card, 1, false, x => x.Is(null, CardType.Unit, x => x.HasAnyCategorie(Categorie.ClanDrummond, Categorie.ClanTuirseach, Categorie.ClanDimun, Categorie.ClanTordarroch, Categorie.ClanHeymaey, Categorie.ClanAnCraite, Categorie.ClanBrokvar)), SelectModeType.MyRow);
            if (target.Count() == 0)
            {
                return 0;
            }
            //所有家族categorie的列表
            var familylist = new List<Categorie>() { Categorie.ClanDrummond, Categorie.ClanTuirseach, Categorie.ClanDimun, Categorie.ClanTordarroch, Categorie.ClanHeymaey, Categorie.ClanAnCraite, Categorie.ClanBrokvar };
            //取出选中卡categorie中的家族部分
            var cate_list = target.First().Status.Categories.Intersect(familylist);
            //可能一张卡是多家族卡
            foreach (var famcat in cate_list)
            {
                var list = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAnyCategorie(famcat) && x != Card);
                foreach (var card in list)
                {
                    await card.Effect.Boost(1, Card);
                }
            }
            return 0;
        }
    }
}
