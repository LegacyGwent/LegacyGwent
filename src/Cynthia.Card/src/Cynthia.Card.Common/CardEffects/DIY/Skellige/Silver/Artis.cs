using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70089")]//亚提斯
    public class Artis : CardEffect
    {//部署：对一个敌军单位造成7点伤害，若摧毁目标，则在对方同排生成一张“巨熊祭品”。
        public Artis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(
                CardId.CultistOblation,
                AnotherPlayer,
                new CardLocation(Card.Status.CardRow, int.MaxValue));

            var selectList = await Game.GetSelectPlaceCards(
                Card,
                filter: x => x.HasAllCategorie(Categorie.Soldier),
                selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Transform(CardId.SvalblodFanatic, Card, isForce: true);
            return 0;
        }
    }
}
