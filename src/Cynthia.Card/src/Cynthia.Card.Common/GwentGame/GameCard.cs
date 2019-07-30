using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public class GameCard : IHasEffects
    {
        public bool IsDead { get => this.CardPoint() <= 0 || Status.Type == CardType.Special||!Status.CardRow.IsOnPlace(); }
        public GameCard(IGwentServerGame game)
        {
            Effects = new EffectSet(this);
            Game = game;
        }

        //通过卡牌状态,效果,所在半场来创建卡牌
        public GameCard(IGwentServerGame game, int playerIndex, CardStatus cardStatus, params string[] effectIds) : this(game)
        {
            PlayerIndex = playerIndex;
            // Effect = cardEffect;
            Status = cardStatus;
            foreach (var effectId in effectIds)
            {
                Effects.Add(game.CreateEffectInstance(effectId, this));
            }
            Effect = game.CreateEffectInstance(effectIds.First(), this);
        }

        //卡牌存在半场
        public int PlayerIndex { get; set; }

        //准备被替换的旧卡牌效果实现
        public CardEffect Effect { get; set; }

        //卡牌的各种信息
        public CardStatus Status { get; set; }

        //卡牌效果合集
        public EffectSet Effects { get; set; }

        //游戏
        public IGwentServerGame Game { get; set; }

        public async Task<TEvent> RaiseEvent<TEvent>(TEvent @event)
        where TEvent : Event
        {
            return await Effects.RaiseEvent(@event);
        }
    }
}