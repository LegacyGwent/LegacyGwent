using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24015")]//狂猎长船
	public class WildHuntDrakkar : CardEffect,IHandlesEvent<AfterUnitDown>
	{//使所有友军“狂猎”单位获得1点增益。 后续出现的其他友军“狂猎”单位也将获得1点增益。
		public WildHuntDrakkar(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            var targets = Game.GetPlaceCards(PlayerIndex).Where(x => x != Card && x.HasAllCategorie(Categorie.WildHunt)).ToList();

            foreach(var target in targets)
            {
                await target.Effect.Boost(1, Card);
            }
			return 0;
		}

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if(@event.Target==Card||!@event.Target.HasAllCategorie(Categorie.WildHunt)||!Card.IsAliveOnPlance())
            {
                return;
            }
            await @event.Target.Effect.Boost(1, Card);
        }
    }
}