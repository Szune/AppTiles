#region License & Terms
// MIT License

// Copyright (c) 2018 Erik Iwarson

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
#endregion
using AppTiles.Attributes;
using AppTiles.Tiles;
using AppTiles.Windows;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AppTiles.Utilities;

namespace AppTiles.Helpers
{
    /* I would like to do this in a less bad way in the future. */
    public static class EditorHelper
    {
        private static readonly Dictionary<Type, EditTileWindow> CachedEditors = new Dictionary<Type, EditTileWindow>();

        public static EditTileWindow GetEditTileWindow(ITile tile)
        {
            var type = tile.GetType();
            if (CachedEditors.ContainsKey(type))
            {
                CachedEditors[type].SetValuesFromProperties(tile);
                return CachedEditors[type];
            }

            CachedEditors[type] = CreateEditTileWindow(tile);
            return CachedEditors[type];
        }

        private static EditTileWindow CreateEditTileWindow(ITile tile)
        {
            return new EditTileWindow(tile);
        }
        
        public static ControlRow GetControlRowFromProperty(PropertyInfo prop,
            ITile tile)
        {
            switch (prop.PropertyType)
            {
                case var stringProp when stringProp.IsAssignableFrom(typeof(string)):
                    return ControlCreator.String(prop, tile);
                case var intProp when intProp.IsAssignableFrom(typeof(int)):
                    return ControlCreator.Int(prop, tile);
                case var colorProp when colorProp.IsAssignableFrom(typeof(Color)):
                    return ControlCreator.Color(prop, tile);
                case var boolProp when boolProp.IsAssignableFrom(typeof(bool)):
                    return ControlCreator.Bool(prop, tile);
                default:
                    throw new InvalidOperationException($"[EditorHelper] Property '{prop.Name}' of type '{prop.PropertyType.Name}' is not supported by {nameof(ShowInEditorAttribute)}");
            }
        }

        /// <summary>
        /// Assigns the specified control's value to the specified property.
        /// </summary>
        /// <param name="property">The property to assign the value to.</param>
        /// <param name="control">The control to get the value from.</param>
        /// <param name="tile">The tile object on which to assign the value.</param>
        public static void SetPropertyValueFromControl(PropertyInfo property, UIElement control, ITile tile)
        {
            if (property.GetCustomAttribute<ShowInEditorAttribute>().IsReadOnly)
                return; // skip trying to assign a value that is supposed to be readonly

            switch (property.PropertyType)
            {
                case var stringProp when stringProp.IsAssignableFrom(typeof(string)):
                    property.SetValue(tile, (control as TextBox).Text);
                    break;
                case var intProp when intProp.IsAssignableFrom(typeof(int)):
                    property.SetValue(tile, int.Parse((control as TextBox).Text));
                    break;
                case var boolProp when boolProp.IsAssignableFrom(typeof(bool)):
                    property.SetValue(tile, (control as CheckBox).IsChecked ?? false); // try not to assign null to a value type
                    break;
                case var colorProp when colorProp.IsAssignableFrom(typeof(Color)):
                    property.SetValue(tile, Colorful.String((control as TextBox).Text));
                    break;
                default:
                    throw new InvalidOperationException($"[EditorHelper] Property '{property.Name}' of type '{property.PropertyType.Name}' is not supported by {nameof(ShowInEditorAttribute)}");
            }
        }


        /// <summary>
        /// Assigns the specified property's value to the specified control.
        /// </summary>
        /// <param name="control">The control to assign the value to.</param>
        /// <param name="property">The property to get the value from.</param>
        /// <param name="tile">The tile object from which to get the value.</param>
        public static void SetControlValueFromProperty(UIElement control, PropertyInfo property, ITile tile)
        {
            switch (property.PropertyType)
            {
                case var stringProp when stringProp.IsAssignableFrom(typeof(string)):
                    (control as TextBox).Text = (string)property.GetValue(tile);
                    break;
                case var intProp when intProp.IsAssignableFrom(typeof(int)):
                    (control as TextBox).Text = ((int)property.GetValue(tile)).ToString();
                    break;
                case var boolProp when boolProp.IsAssignableFrom(typeof(bool)):
                    (control as CheckBox).IsChecked = (bool) property.GetValue(tile);
                    break;
                case var colorProp when colorProp.IsAssignableFrom(typeof(Color)):
                    (control as TextBox).Text = ((Color) property.GetValue(tile)).ToString();
                    break;
                default:
                    throw new InvalidOperationException($"[EditorHelper] Property '{property.Name}' of type '{property.PropertyType.Name}' is not supported by {nameof(ShowInEditorAttribute)}");
            }
        }
    }
}
