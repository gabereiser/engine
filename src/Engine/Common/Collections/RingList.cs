using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reactor.Common.Collections
{
    public class RingList<T> : List<T>
    {
        public int Limit { get; set; }

        public RingList(int limit) : base()
        {
            Limit = limit;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void Add(T item)
        {
            if (Count >= Limit)
                RemoveAt(0);
            base.Add(item);
        }
    }
}