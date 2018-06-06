using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AppTiles.Helpers
{
    // ReSharper disable once InconsistentNaming -> needs consistency :(
    public static class UIElementCollectionExtensions
    {
        public static void AddRange(this UIElementCollection collection, IEnumerable<UIElement> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
