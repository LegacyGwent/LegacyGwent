using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70012")]//陶森特游侠骑士

    public class ToussaintKnightErrant : CardEffect
    {//部署：己方手牌每比对手少1张，便获得一次4点增益。
        public ToussaintKnightErrant(GameCard card) : base(card) { }
        private const int boostPoint = 4;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int myHandCardCount = Game.PlayersHandCard[PlayerIndex].Count();
            int opponentHandCardCount = Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)].Count();
            await Game.Debug(myHandCardCount.ToString());
            await Game.Debug(opponentHandCardCount.ToString());
            int boostTimes = 0;
            if (opponentHandCardCount > myHandCardCount)
            {
                boostTimes = opponentHandCardCount - myHandCardCount;
            }

            for (int i = 0; i < boostTimes; i++)
            {
                await Card.Effect.Boost(boostPoint, Card);
            }
            return 0;
        }
    }
}
