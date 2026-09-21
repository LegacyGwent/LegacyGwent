using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70193")] //史裘达尔·德拉蒙德
    public class SkjordalDrummond : CardEffect
    {
        //部署：部署：从牌组中丢弃1张铜色家族单位牌，然后复活1个不同家族的铜色单位。。
        //  Deploy: Discard a Bronze Clan unit from your deck, then resurrect a Bronze unit from a different Clan.
        public SkjordalDrummond(GameCard card) : base(card) {}

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var familylist = new List<Categorie>() { Categorie.ClanDrummond, Categorie.ClanTuirseach, Categorie.ClanDimun, Categorie.ClanTordarroch, Categorie.ClanHeymaey, Categorie.ClanAnCraite, Categorie.ClanBrokvar };
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardUseInfo == CardUseInfo.MyRow && familylist.Any(family => x.HasAnyCategorie(family))).Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择丢弃一张牌");
            if (result.Count() == 0)
            {
                return 0;
            }
            var discardtarget = result.First();
            await discardtarget.Effect.Discard(Card);
            
            //取出选中卡categorie中的家族部分
            var cate_list = discardtarget.Status.Categories.Intersect(familylist);
            if (cate_list.Count() == 0)
            {
                return 0;
            }
            var clan = cate_list.FirstOrDefault();


            var reslist = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit && !x.HasAnyCategorie(clan) && familylist.Any(family => x.HasAnyCategorie(family))  && x.Status.CardId != discardtarget.Status.CardId).ToList();
            if (reslist.Count() == 0)
            {
                return 0;
            }

            //让玩家选择一张卡
            var resresult = await Game.GetSelectMenuCards
            (Card.PlayerIndex, reslist, 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (resresult.Count() == 0)
            {
                return 0;
            }
            //复活到玩家指定位置
            await resresult.First().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 0;
        }
    }
}