namespace Cynthia.Card
{
    //卡牌落下后
    public class AfterUnitDown : Event
    {
        public GameCard Target { get; set; }

        //是否来自场上
        public bool IsFromPlance { get; set; }

        public bool IsFromHand { get; set; }

        public bool IsSpying { get; set; }

        // True only when the unit entered through CardEffect.Play. Summon,
        // Resurrect, Move and other placement paths leave this false.
        public bool IsPlayed { get; set; }

        public (bool isMove, bool isFromeEnemy) IsMoveInfo { get; set; }

        public bool IsFromAnother { get => !IsFromHand && !IsFromPlance && !IsMoveInfo.isMove; }

        public AfterUnitDown(GameCard target, bool isFromHand, bool isFromPlance, (bool isMove, bool isFromEnemy) isMoveInfo, bool isSpying, bool isPlayed = false)
        {
            IsSpying = isSpying;
            IsFromHand = isFromHand;
            IsFromPlance = isFromPlance;
            Target = target;
            IsMoveInfo = isMoveInfo;
            IsPlayed = isPlayed;
        }
    }
}
