using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70070")]//路途埋伏 Highwaymen
    public class Highwaymen : CardEffect
    {//力竭。若己方起始牌组仅有铜色牌，则生成1张“史帕拉流亡军”，并将2张“史帕拉流亡军”加入牌组。（史帕拉流亡军，6战力，佚亡，检视己方牌组中3张铜色非同名单位牌，随后打出1张。）
        public Highwaymen(GameCard card) : base(card) { }
        public bool IsUse { get; set; } = false;  
        public override async Task<int> CardUseEffect()
        {
            if (IsUse || (Game.PlayerBaseDeck[PlayerIndex].Deck.Any(x => x.IsAnyGroup(Group.Gold,Group.Silver))))
            {
                return 0;
            }
            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            await Game.CreateCard(CardId.StraysofSpalla, PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            for (var i = 0; i < 2; i++)
            {
                await Game.CreateCard(CardId.StraysofSpalla, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)));
            }
            return 1;
        }
    }
}