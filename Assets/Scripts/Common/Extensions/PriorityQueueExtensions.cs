using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Common.Extensions
{
    public static class PriorityQueueExtensions
    {
        public static bool Contains<TElement, TPriority>(this PriorityQueue<TElement, TPriority> queue, TElement element)
        {
            return queue.UnorderedItems.Any(item => EqualityComparer<TElement>.Default.Equals(item.Element, element));
        }
    }
}