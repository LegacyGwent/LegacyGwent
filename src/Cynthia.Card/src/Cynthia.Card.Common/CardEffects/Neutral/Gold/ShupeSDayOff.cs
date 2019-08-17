using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12041")]//店店的大冒险
	public class ShupeSDayOff : CardEffect
	{//若己方起始牌组没有重复牌，则派“店店”去冒险。
		public ShupeSDayOff(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
            if (HaveRepeatedCopper())
            {
                return 0;
            }
            return await Card.CreateAndMoveStay(CardId.ShupeMage, CardId.ShupeHunter, CardId.ShupeKnight);

        }

        private bool HaveRepeatedCopper()
        {
            return (Game.PlayerBaseDeck[PlayerIndex].Deck
                .GroupBy(x => x.CardId).Any(x => x.Count() != 1));

        }
    }
}