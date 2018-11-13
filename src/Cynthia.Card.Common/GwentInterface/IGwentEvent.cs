using System.Threading.Tasks;

namespace Cynthia.Card
{
    public interface IGwentEvent
    {
        Task OnTurnStart(int playerIndex);//谁的回合开始了
        Task OnTurnOver(int playerIndex);//谁的回合结束了
        Task OnRoundOver(int RoundCount, int player1Point, int player2Point);//第几轮,谁赢了
        Task OnPlayerPass(int playerIndex);
        Task OnPlayerDraw(int playerIndex, GameCard taget);//抽卡
        Task OnCardReveal(GameCard taget, GameCard source = null);//揭示
        Task OnCardConsume(GameCard master, GameCard taget);//吞噬
        Task OnCardBoost(GameCard taget, int num, GameCard source = null);//增益
        Task OnCardHurt(GameCard taget, int num, GameCard source = null);//受伤
        Task OnSpecialPlay(GameCard taget);//法术卡使用前
        Task OnUnitPlay(GameCard taget);//单位卡执行一段部署前
        Task OnUnitDown(GameCard taget);//单位卡落下时(二段部署前)
        Task OnCardDeath(GameCard taget,CardLocation source);//有卡牌进入墓地
        Task OnCardSpyingChange(GameCard taget, bool isSpying, GameCard source = null);//场上间谍改变
        Task OnCardDiscard(GameCard taget, GameCard source = null);//卡牌被丢弃
        Task OnCardAmbush(GameCard taget);//有伏击卡触发
        Task OnCardSwap(GameCard taget, GameCard source = null);//卡牌交换
        Task OnCardConceal(GameCard taget, GameCard source = null);//隐匿
        Task OnCardLockChange(GameCard taget, bool isLock, GameCard source = null);//锁定状态改变
        Task OnCardAddArmor(GameCard taget, int num, GameCard source = null);//增加护甲
        Task OnCardSubArmor(GameCard taget, int num, GameCard source = null);//降低护甲
        Task OnCardArmorBreak(GameCard taget, GameCard source = null);//护甲被破坏
        Task OnCardResurrect(GameCard taget, GameCard source = null);//有卡牌复活
        Task OnCardResilienceChange(GameCard taget, bool isResilience, GameCard source = null);//坚韧状态改变
        Task OnWeatherApply(int playerIndex, int row, RowStatus type);//有天气降下
        Task OnCardHeal(GameCard taget, GameCard source = null);//卡牌被治愈
        Task OnCardReset(GameCard taget, GameCard source = null);//卡牌被重置
        Task OnCardStrengthen(GameCard taget, int num, GameCard source = null);//强化
        Task OnCardWeaken(GameCard taget, int num, GameCard source = null);//削弱
        Task OnCardDrain(GameCard master, int num, GameCard taget);//有单位汲食时
        Task OnCardCharm(GameCard taget, GameCard source = null);//被魅惑
        Task OnCardMove(GameCard taget, GameCard source = null);//位移时
    }
}