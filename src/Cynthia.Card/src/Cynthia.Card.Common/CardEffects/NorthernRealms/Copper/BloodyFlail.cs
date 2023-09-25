using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44034")]//染血连枷
    public class BloodyFlail : CardEffect
    {//造成5点伤害，并在随机排生成1只“鬼灵”。
        public BloodyFlail(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //下面代码基于:鬼灵生成在随机排最右
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(5, Card);
            //最右生成
            if (Game.GetRandomRow(PlayerIndex, out var rowIndex))
            {
                await Game.CreateCardAtEnd(CardId.Specter, PlayerIndex, Game.GetRandomCanPlayLocation(Card.PlayerIndex,true).RowPosition);
            }
            return 0;
        }
    }
}