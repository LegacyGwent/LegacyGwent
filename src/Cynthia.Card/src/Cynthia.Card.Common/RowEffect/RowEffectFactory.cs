using System;
using System.Collections.Generic;
using System.Text;
using Autofac.Core.Activators;
namespace Cynthia.Card.Common.RowEffect
{
    public class RowEffectFactory : IDisposable
    {
        private static RowEffectFactory _instance;

        private Dictionary<RowStatus, Card.RowEffect> _dictionary;

        public static RowEffectFactory Instance
        {
            get { return _instance ??= new RowEffectFactory(); }
        }

        private RowEffectFactory()
        {
            Init();
        }

        public Card.RowEffect GetRowEffectByRowStatus(RowStatus status)
        {
            Card.RowEffect res = null;
            if (_dictionary.TryGetValue(status, out res))
            {
                return (Card.RowEffect)res.Clone();
            }
            return new NoneStatus();
        }

        private void Init()
        {
            _dictionary = new Dictionary<RowStatus, Card.RowEffect>();
            Add(new BitingFrostStatus());
            Add(new BloodMoonStatus());
            Add(new DragonDreamStatus());
            Add(new FullMoonStatus());
            Add(new GoldenFrothStatus());
            Add(new ImpenetrableFogStatus());
            Add(new KorathiHeatwaveStatus());
            Add(new NoneStatus());
            Add(new PitTrapStatus());
            Add(new RaghNarRoogStatus());
            Add(new SkelligeStormStatus());
            Add(new TorrentialRainStatus());

        }

        private void Add(Card.RowEffect rowEffect)
        {
            _dictionary[rowEffect.StatusType] = rowEffect;
        }

        public void Dispose()
        {
            _dictionary.Clear();
        }
    }
}
