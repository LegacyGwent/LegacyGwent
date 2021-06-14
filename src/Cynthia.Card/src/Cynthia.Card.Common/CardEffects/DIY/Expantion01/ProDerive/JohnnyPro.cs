using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130080")]//乔尼：乔尼
    public class JohnnyPro : CardEffect
    {//丢弃1张手牌，并在手牌中创造1张对方起始牌组中颜色相同的原始同名牌。
        public JohnnyPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersHandCard[PlayerIndex]);
            if(!cards.TrySingle(out var target))
            {
                return 0;
            }
            var targetGroup = target.Status.Group;
            if(!Game.PlayerBaseDeck[AnotherPlayer].Deck.Where(x=>x.Group==targetGroup).Select(x=>x.CardId).TryMessOne(out var targetId,Game.RNG))
            {
                return 0;
            }
            await target.Effect.Discard(Card);


            var list = Game.PlayerBaseDeck[AnotherPlayer].Deck.Where(x=>x.Group==targetGroup)
                .Mess(RNG).Take(3).Select(x => x.CardId)
                .ToList();

            var selectList = list.Select(x => new CardStatus(x)).ToList();
            var result = (await Game.GetSelectMenuCards(PlayerIndex, selectList, isCanOver: false, title: "选择一张牌")).Reverse().ToList();
            //先选的先打出u 
            if (result.Count() <= 0) return 0;
            foreach (var CardIndex in result)
            {
                await Game.CreateCardAtEnd(selectList[CardIndex].CardId, PlayerIndex, RowPosition.MyHand);
            }
            return 0;
        }
    }
}