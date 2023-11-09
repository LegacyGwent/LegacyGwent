using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70098")]//维里赫德旅破坏者
    public class VriheddSaboteur : CardEffect
    {//剩余卡组中每有一张精灵标签单位卡便获得1点增益
        public VriheddSaboteur (GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var listElf = Game.PlayersDeck[PlayerIndex].Where(x => x.HasAnyCategorie(Categorie.Elf)).ToList();
            int boostNum = listElf.Count();

            await Card.Effect.Boost(boostNum, Card);
            return 0;
        }
    }
}