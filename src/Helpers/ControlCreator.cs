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
using System;
using AppTiles.Attributes;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AppTiles.Tiles;
using AppTiles.Windows;

namespace AppTiles.Helpers
{
    /* I would like to do this in a less bad way in the future. */
    public static class ControlCreator
    {
        public const int ControlHeight = 27;

        #region Specific types
        public static (FrameworkElement left, FrameworkElement right) String(PropertyInfo prop, ITile tile)
        {
            var label = Label(prop);
            var textBox = TextBox((string) prop.GetValue(tile), prop);
            return (label, textBox);
        }

        public static (FrameworkElement left, FrameworkElement right) Bool(PropertyInfo prop, ITile tile)
        {
            var label = Label(prop);
            var checkBox = CheckBox((bool)prop.GetValue(tile), prop);
            return (label, checkBox);
        }

        public static (FrameworkElement left, FrameworkElement right) Int(PropertyInfo prop, ITile tile)
        {
            var label = Label(prop);
            var textBox = TextBox(prop.GetValue(tile).ToString(), prop);
            return (label, textBox);
        }

        public static (FrameworkElement left, FrameworkElement right) Color(PropertyInfo prop, ITile tile)
        {
            var label = Label(prop);
            var button = new Button
            {
                Content = "...",
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 15,
                Width = 15,
                Padding = new Thickness(0, -6, 0, 0),
                Margin = new Thickness(0, 0, 5, 0),
            };
            button.Click += ColorPickerButton_Click;
            var left = new Grid();
            left.Children.AddRange(new UIElement[] {label, button});
            var right = TextBox(prop.GetValue(tile).ToString(), prop);
            button.Tag = (prop, right);
            return (left, right);
        }
        #endregion

        #region Specific controls
        public static Label Label(PropertyInfo prop)
        {
            var displayText = GetDisplayText(prop);
            return new Label
            {
                Content = displayText + ":",
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = ControlHeight
            };
        }

        public static TextBox TextBox(string text, PropertyInfo prop)
        {
            return new TextBox
            {
                IsReadOnly = prop.GetCustomAttribute<ShowInEditorAttribute>().IsReadOnly,
                Text = text,
                Height = ControlHeight,
                Tag = prop,
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }

        public static CheckBox CheckBox(bool isChecked, PropertyInfo prop)
        {
            return new CheckBox
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                IsChecked = isChecked,
                Height = ControlHeight,
                Tag = prop
            };
        }
        #endregion

        private static void ColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            if(!(sender is FrameworkElement element)) 
                throw new InvalidOperationException($"{nameof(sender)} of type {sender.GetType()} does not inherit {nameof(FrameworkElement)}.");
            (PropertyInfo prop, TextBox txt) = (ValueTuple<PropertyInfo, TextBox>)element.Tag;

            var displayText = prop.GetCustomAttribute<ShowInEditorAttribute>().DisplayText;
            var pickedColor = TryGetColorFromString(txt.Text, out var color)
                ? ColorPickerWindow.ShowDialog(displayText, color)
                : ColorPickerWindow.ShowDialog(displayText);

            if(pickedColor != null)
                txt.Text = pickedColor.ToString(); 
        }

        private static bool TryGetColorFromString(string hex, out Color color)
        {
            try
            {
                color = Colorful.String(hex);
                return true;
            }
            catch
            {
                color = Colors.White;
                return false;
            }
        }

        private static string GetDisplayText(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<ShowInEditorAttribute>().DisplayText;
        }
    }
}
