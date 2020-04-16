using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70017")]//辛特拉战地医师
    public class CintrianFieldMedic : CardEffect
    {//部署：将 1 个非同名友军铜色单位洗回牌组，然后从牌库打出 1 张随机铜色单位牌。

        public CintrianFieldMedic(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow, filter: x => x.Is(Group.Copper, CardType.Unit, filter: x => x.Status.CardId != Card.Status.CardId));
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            target.Effect.Repair(true);
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count)), target);

            if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Is(Group.Copper, CardType.Unit)).TryMessOne(out var playCard, Game.RNG))
            {
                return 0;
            }
            await playCard.MoveToCardStayFirst();
            return 1;
        }
    }
}
