using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("52002")]//萨琪亚萨司
    public class Saesenthessis : CardEffect
    {//增益自身等同于友军和手牌中“矮人”单位数量；造成等同于友军和手牌中“精灵”单位数量的伤害。
        public Saesenthessis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var listDwarf = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAnyCategorie(Categorie.Dwarf)).ToList();
            var handDwarf = Game.PlayersHandCard[PlayerIndex].FilterCards(filter: x => x.HasAnyCategorie(Categorie.Dwarf)).ToList();

            int boostNum = listDwarf.Count() + handDwarf.Count();

            await Card.Effect.Boost(boostNum, Card);

            var listElf = Game.GetPlaceCards(Card.PlayerIndex).FilterCards(filter: x => x.HasAnyCategorie(Categorie.Elf)).ToList();
            var handElf = Game.PlayersHandCard[PlayerIndex].FilterCards(filter: x => x.HasAnyCategorie(Categorie.Elf)).ToList();

            int damageNum = listElf.Count() + handElf.Count();

            if (damageNum == 0)
            {
                return 0;
            }

            var selectList = await Game.GetSelectPlaceCards(Card, 1);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Damage(damageNum, Card);



            return 0;
        }
    }
}