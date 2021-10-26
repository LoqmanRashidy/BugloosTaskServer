using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Extension
{
    public static class FlattenExtension
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T,IEnumerable<T>> childrenSelector)
        {
            foreach (var item in source)
            {
                yield return item;
                foreach (var child in childrenSelector(item).Flatten(childrenSelector))
                {
                    yield return child;
                }
            }
        }
    }
}
