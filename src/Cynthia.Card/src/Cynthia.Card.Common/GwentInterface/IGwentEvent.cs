using System.Threading.Tasks;

namespace Cynthia.Card
{
    public interface IGwentEvent
    {
        Task OnTurnStart(int playerIndex);//谁的回合开始了
        Task OnTurnOver(int playerIndex);//谁的回合结束了
        Task OnRoundOver(int RoundCount, int player1Point, int player2Point);//第几轮,谁赢了
        Task OnPlayerPass(int playerIndex);
        Task OnPlayerDraw(int playerIndex, GameCard target);//抽卡
        Task OnCardReveal(GameCard target, GameCard source = null);//揭示
        Task OnCardConsume(GameCard master, GameCard target);//吞噬
        Task OnCardBoost(GameCard target, int num, GameCard source = null);//增益
        Task OnCardHurt(GameCard target, int num, GameCard source = null);//受伤
        Task OnSpecialPlay(GameCard target);//法术卡使用前
        Task OnUnitPlay(GameCard target);//单位卡执行一段部署前
        Task OnUnitDown(GameCard target);//单位卡落下时(二段部署前)
        Task OnCardDeath(GameCard target, CardLocation source);//有死亡墓地
        Task OnCardToCemetery(GameCard target, CardLocation source);//有卡牌进入墓地
        Task OnCardSpyingChange(GameCard target, bool isSpying, GameCard source = null);//场上间谍改变
        Task OnCardDiscard(GameCard target, GameCard source = null);//卡牌被丢弃
        Task OnCardAmbush(GameCard target);//有伏击卡触发
        Task OnCardSwap(GameCard target, GameCard source = null);//卡牌交换
        Task OnCardConceal(GameCard target, GameCard source = null);//隐匿
        Task OnCardLockChange(GameCard target, bool isLock, GameCard source = null);//锁定状态改变
        Task OnCardAddArmor(GameCard target, int num, GameCard source = null);//增加护甲
        Task OnCardSubArmor(GameCard target, int num, GameCard source = null);//降低护甲
        Task OnCardArmorBreak(GameCard target, GameCard source = null);//护甲被破坏
        Task OnCardResurrect(GameCard target, GameCard source = null);//有卡牌复活
        Task OnCardResilienceChange(GameCard target, bool isResilience, GameCard source = null);//坚韧状态改变
        Task OnWeatherApply(int playerIndex, int row, RowStatus type);//有天气降下
        Task OnCardHeal(GameCard target, GameCard source = null);//卡牌被治愈
        Task OnCardReset(GameCard target, GameCard source = null);//卡牌被重置
        Task OnCardStrengthen(GameCard target, int num, GameCard source = null);//强化
        Task OnCardWeaken(GameCard target, int num, GameCard source = null);//削弱
        Task OnCardDrain(GameCard master, int num, GameCard target);//有单位汲食时
        Task OnCardCharm(GameCard target, GameCard source = null);//被魅惑
        Task OnCardMove(GameCard target, GameCard source = null);//位移时
    }
}