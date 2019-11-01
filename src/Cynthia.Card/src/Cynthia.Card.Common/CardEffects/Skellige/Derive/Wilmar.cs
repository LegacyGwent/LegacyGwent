using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("65007")]//威尔玛
    public class Wilmar : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：若为对方回合，则在对面此排生成1头“熊”。
        public Wilmar(GameCard card) : base(card) { }

        // public override async Task CardDownEffect(bool isSpying, bool isReveal)
        // {
        //     await Game.Debug($"生成熊的小骷髅落地了,它属于玩家{Card.PlayerIndex}");
        // }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            // if (@event.Target == Card)
            // {
            //     await Game.Debug($"生成熊的小骷髅死亡了,当前是玩家{Game.GameRound.ToPlayerIndex(Game)}的回合,小骷髅属于玩家{Card.PlayerIndex},因此 小骷髅{(Game.GameRound.ToPlayerIndex(Game) == Card.PlayerIndex ? "不" : "")}生成熊。");
            // }
            //如果死的不是本卡或flag为false
            if (@event.Target != Card || Game.GameRound.ToPlayerIndex(Game) == PlayerIndex)
            {
                return;
            }
            if (Game.RowToList(AnotherPlayer, @event.DeathLocation.RowPosition).Count >= Game.RowMaxCount)
            {
                // await Game.Debug($"要生成熊的那一排,目前有{Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count}个单位,但游戏设定每排最多只能有{Game.RowMaxCount}个单位,因此无法生成熊。");
                return;
            }

            // await Game.Debug("生成了熊");
            await Game.CreateCardAtEnd(CardId.Bear, AnotherPlayer, @event.DeathLocation.RowPosition);
            return;
        }
    }
}