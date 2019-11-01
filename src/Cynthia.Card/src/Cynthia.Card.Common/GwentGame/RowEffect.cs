using System.Linq;
using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    public abstract class RowEffect : Effect, ICloneable
    {
        public int PlayerIndex { get => Row.PlayerIndex; }

        public RowPosition RowPosition { get => Row.RowPosition; }

        public IList<GameCard> RowCards { get => Row?.RowCards; }
        public IList<GameCard> AliveNotConceal { get => Row?.RowCards.Where(x => x.IsAliveOnPlance() && !x.Status.Conceal).ToList(); }

        public GameRow Row { get; set; }

        public IGwentServerGame Game { get => Row?.Game; }

        public abstract RowStatus StatusType { get; }
        public object Clone()
        {
            RowEffect copy = (RowEffect)this.MemberwiseClone();
            return copy;
        }
    }
}