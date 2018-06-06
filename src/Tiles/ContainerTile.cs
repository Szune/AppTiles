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
using System.Windows.Media;
using AppTiles.Attributes;
using AppTiles.Windows;

namespace AppTiles.Tiles
{
    public class ContainerTile : TileBase
    {
        [ShowInEditor]
        public int Columns
        {
            get => Children?.Columns ?? -1;
            set
            {
                if (Children != null)
                    Children.Columns = value;
            }
        }

        [ShowInEditor]
        public int Rows
        {
            get => Children?.Rows ?? -1;
            set
            {
                if (Children != null)
                    Children.Rows = value;
            }
        }

        [ShowInEditor]
        public int Width
        {
            get => Children?.Width ?? -1;
            set
            {
                if (Children != null)
                    Children.Width = value;
            }
        }

        [ShowInEditor]
        public int Height
        {
            get => Children?.Height ?? -1;
            set
            {
                if (Children != null)
                    Children.Height = value;
            }
        }
        public TileCollection Children { get; set; }

        public ContainerTile(int id, int column, int row, string text, Color background, Color foreground, TileCollection children) 
            : base(id, column, row, text, background, foreground)
        {
            Children = children;
            if (Children != null)
                Children.ParentText = text;
            TextChanged += ContainerTile_TextChanged;
        }

        private void ContainerTile_TextChanged(object sender, EventArgs e)
        {
            if(Children != null)
                Children.ParentText = Text;
        }

        public override void Execute()
        {
            var window = new TileCollectionWindow(Children);
            window.Show();
        }

        public override void Reset()
        {
            Children = new TileCollection();
            if (Children != null)
                Children.ParentText = Text;
            base.Reset();
        }

    }
}
