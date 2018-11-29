namespace Cynthia.Card
{
    [CardEffectId("None")]
    public class NoneEffect : CardEffect
    {
        public NoneEffect(IGwentServerGame game, GameCard card) : base(game, card){}
    }
}