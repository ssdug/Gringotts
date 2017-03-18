using System;
using System.Collections.Generic;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class EnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }

        public static IEnumerable<T> Flatten<T>(this T node, Func<T, IEnumerable<T>> selector)
        {
            var stack = new Stack<T>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                foreach (var child in selector(current))
                {
                    stack.Push(child);
                }
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            foreach (var item in source)
            {
                yield return item;

                var children = childrenSelector(item);
                if (children == null)
                    continue;

                foreach (var child in children.Flatten(childrenSelector))
                    yield return child;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
    }
}