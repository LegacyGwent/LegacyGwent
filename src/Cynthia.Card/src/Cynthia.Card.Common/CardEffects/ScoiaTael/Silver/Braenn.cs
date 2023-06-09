using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53010")]//布蕾恩
    public class Braenn : CardEffect
    {//对1个敌军单位造成等同于自身战力的伤害。若目标被摧毁，则使位于手牌、牌组和己方半场除自身外所有“树精”和伏击单位获得1点增益。
        public Braenn(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            // 获取自身战斗力
            var selfPoint = Card.CardPoint();
            // var isBoost = Game.GameRowEffect[target.PlayerIndex][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost;
            await target.Effect.Damage(selfPoint, Card);

            // 如果目标被杀
            var isBoost = target.IsDead;
            if (isBoost)
            {

                var cards = Game.PlayersHandCard[PlayerIndex].Concat(Game.GetPlaceCards(PlayerIndex, isHasConceal: true)).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => (x.HasAnyCategorie(Categorie.Dryad) || x.HasAnyCategorie(Categorie.Ambush) || x.Status.Conceal || x.Status.IsConcealCard) && x != Card);

                foreach (var card in cards)
                {
                    await card.Effect.Strengthen(1, Card);
                }
                return 0;
            }
            return 0;



        }
    }
}