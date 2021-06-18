using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130040")]//爱丽丝的同伴：晋升
    public class IrisCompanionsPro : CardEffect
    {//将1张牌从牌组、己方半场或墓地移至手牌，然后随机丢弃1张牌。
        public IrisCompanionsPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, Game.PlayersHandCard[Card.PlayerIndex], 1, isCanOver: false);
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var target in cards)
            {
                await target.Effect.Discard(Card);
            }
            var list = Game.PlayersDeck[PlayerIndex].Mess(RNG).ToList();
            //让玩家选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(PlayerIndex, list, isCanOver: false);
            if (result.Count == 0) return 0;//如果没有任何符合标准的牌,返回
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(PlayerIndex);//抽卡
                                                    //---------------------------------------------------------------------------
                                                    //随机弃掉一张
            //选择选项,设置每个选项的名字和效果
            /*
            var switchCard = await Card.GetMenuSwitch
            (
                ("牌组", "将1张牌从牌组移至手牌，然后随机丢弃1张牌。"),
                ("己方半场", "将1张牌从己方半场移至手牌，然后随机丢弃1张牌。"),
                ("墓地", "将1张牌从墓地移至手牌，然后随机丢弃1张牌。")
            );
            if (switchCard == 0)//牌组
            {
                //己方卡组乱序呈现
                var list = Game.PlayersDeck[PlayerIndex].Mess(RNG).ToList();
                //让玩家选择一张卡,不能不选
                var result = await Game.GetSelectMenuCards(PlayerIndex, list, isCanOver: false);
                if (result.Count == 0) return 0;//如果没有任何符合标准的牌,返回
                var dcard = result.Single();
                var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
                await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
                await Game.PlayerDrawCard(PlayerIndex);//抽卡
                                                    //---------------------------------------------------------------------------
                                                    //随机弃掉一张
            }
            else if (switchCard == 1)//半场
            {
                var result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
                if (result.Count() > 0)
                {
                    result.Single().Effect.Repair(true);
                    await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyHand, CardIndex = 0 }, result.Single(), refreshPoint: true);
                }
            }
            else if (switchCard == 2)//墓地
            {
                var list = Game.PlayersCemetery[Card.PlayerIndex].Where(x=>!x.HasAllCategorie(Categorie.Leader)).ToList();
                //让玩家选择
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, isCanOver: true);
                if (result.Count() != 0)
                {
                    var playerIndex = result.Single().PlayerIndex;
                    result.Single().Effect.Repair();
                    await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyHand, CardIndex = 0 }, result.Single(), refreshPoint: true);
                }
            }
            await Game.PlayersHandCard[PlayerIndex].Mess(RNG).First().Effect.ToCemetery();
            */
            return 0;
        }
    }
}