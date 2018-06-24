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
using AppTiles.Controls;
using Newtonsoft.Json;
using System.Windows.Media;

namespace AppTiles.Tiles
{
    public abstract class TileBase : ITile
    {
        [ShowInEditor(IsReadOnly = true, IsBaseClass = true)]
        public int Id { get; }
        [ShowInEditor(IsBaseClass = true)]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                Button?.UpdateText(_text);
                TextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler TextChanged;

        private string _text;
        public int Column { get; }
        public int Row { get; }
        public abstract void Execute();


        [ShowInEditor(IsBaseClass = true)]
        public Color Background
        {
            get => _background;
            set
            {
                _background = value;
                if(Button != null)
                    Button.Background = new SolidColorBrush(value);
            }
        }
        private Color _background;

        [ShowInEditor(IsBaseClass = true)]
        public Color Foreground
        {
            get => _foreground;
            set
            {
                _foreground = value;
                if(Button != null)
                    Button.Foreground = new SolidColorBrush(value);
            }
        }
        private Color _foreground;

        [JsonIgnore]
        public TileButton Button { get; private set; }

        protected TileBase(int id, int column, int row, string text, Color background, Color foreground)
        {
            Id = id;
            Column = column;
            Row = row;
            Text = text;
            Background = background;
            Foreground = foreground;
        }

        public void SetButton(TileButton button)
        {
            Button = button;
            Button.RefreshCommands();
        }

        public virtual void Reset()
        {
            Text = "-";
            Background = Colors.Black;
            Foreground = Colors.White;
        }

        public override string ToString()
        {
            return $"[{GetType()} ({Id})] {Text}";
        }
    }
}
