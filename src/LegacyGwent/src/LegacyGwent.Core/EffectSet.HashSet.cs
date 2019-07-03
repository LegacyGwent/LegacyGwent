using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LegacyGwent
{
    public partial class EffectSet : ICollection<Effect>, IReadOnlyCollection<Effect>, IDeserializationCallback, ISerializable
    {
        private readonly HashSet<Effect> _data = new HashSet<Effect>();

        public int Count => _data.Count;

        public bool IsReadOnly => ((ICollection<Effect>)_data).IsReadOnly;


        public bool Contains(Effect item) => _data.Contains(item);

        public void CopyTo(Effect[] array, int arrayIndex) => _data.CopyTo(array, arrayIndex);

        public IEnumerator<Effect> GetEnumerator() => ((ICollection<Effect>)_data).GetEnumerator();

        public void GetObjectData(SerializationInfo info, StreamingContext context) => _data.GetObjectData(info, context);

        public void OnDeserialization(object sender) => _data.OnDeserialization(sender);

        IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((ICollection<Effect>)_data).GetEnumerator();
    }
}