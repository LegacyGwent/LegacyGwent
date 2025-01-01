using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12027")]//狐妖
    public class Aguara : CardEffect
    {//择二：使最弱的友军单位获得5点增益；使手牌中的1个随机单位获得5点增益；对最强的1个敌军单位造成5点伤害；魅惑1个战力不高于5点的敌军“精灵”单位。
        public Aguara(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于 本卡不会buff自己
            var cards = new (string title, string message)[] { ("超然之力", "使最弱的友军单位获得5点增益。"), ("幻象", "使手牌中的1个随机单位获得5点增益。"), ("狂热攻势", "对最强的1个敌军单位造成5点伤害。"), ("诱拐", "魅惑1个战力不高于5点的敌军“精灵”单位") };
            var cardList = cards.Select(x => new CardStatus(Card.Status.CardId) { Name = x.title, Info = x.message }).ToList();
            var switchCard = await Card.Effect.Game.GetSelectMenuCards(Card.PlayerIndex, cardList, selectCount: 2, title: "选择两个选项");

            foreach (var i in switchCard)
            {
                switch (i)
                {
                    case 0:
                        {
                            await buffplace0();
                            break;
                        }
                    case 1:
                        {
                            await buffhand2();
                            break;
                        }
                    case 2:
                        {
                            await damageplace1();
                            break;
                        }
                    case 3:
                        {
                            await charmplace3();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }
            return 0;

        }

        private async Task<int> buffplace0()
        {
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex && x != Card).WhereAllLowest().Mess(RNG).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            await cards.First().Effect.Boost(5, Card);
            return 0;
        }

        private async Task<int> damageplace1()
        {
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex).WhereAllHighest().Mess(RNG).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            await cards.First().Effect.Damage(5, Card);
            return 0;
        }

        private async Task<int> buffhand2()
        {
            var cards = Game.PlayersHandCard[Card.PlayerIndex].Where(x => x.Status.Type == CardType.Unit && x.CardInfo().CardUseInfo == CardUseInfo.MyRow).Mess(RNG).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            await cards.First().Effect.Boost(5, Card);
            return 0;
        }

        private async Task<int> charmplace3()
        {
            if (!Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.HasAllCategorie(Categorie.Elf) && x.CardPoint() <= 5).TryMessOne(out var target, Game.RNG))
            {
                return 0;
            }
            await target.Effect.Charm(Card);
            return 0;
        }

    }
}
