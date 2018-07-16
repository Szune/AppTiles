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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AppTiles.Tiles
{
    [JsonObject(MemberSerialization.Fields)]
    public class TileCollection : IList<ITile>
    {
        private int _columns;
        private int _rows;
        private int _width;
        private int _height;
        private string _parentText;
        private readonly List<ITile> _tiles;

        public const int DefaultColumnAmount = 2;
        public const int DefaultRowAmount = 2;
        public const int DefaultWidth = 350;
        public const int DefaultHeight = 250;

        public int Columns
        {
            get => _columns;
            set => _columns = value;
        }

        public int Rows
        {
            get => _rows;
            set => _rows = value;
        }

        public int Width
        {
            get => _width;
            set => _width = value;
        }

        public int Height
        {
            get => _height;
            set => _height = value;
        }

        public string ParentText
        {
            get => _parentText;
            set => _parentText = value;
        }

        public TileCollection()
        {
            _tiles = new List<ITile>();
            _columns = DefaultColumnAmount;
            _rows = DefaultRowAmount;
            _width = DefaultWidth;
            _height = DefaultHeight;
        }

        public TileCollection(int columns, int rows, int width, int height, List<ITile> tiles, string parentText)
        {
            _tiles = tiles;
            _parentText = parentText;
            _columns = columns;
            _rows = rows;
            _width = width;
            _height = height;
        }

        public IEnumerator<ITile> GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        public void Add(ITile item)
        {
            _tiles.Add(item);
        }

        public void Clear()
        {
            _tiles.Clear();
        }

        public bool Contains(ITile item)
        {
            return _tiles.Contains(item);
        }

        public void CopyTo(ITile[] array, int arrayIndex)
        {
            _tiles.CopyTo(array, arrayIndex);
        }

        public bool Remove(ITile item)
        {
            return _tiles.Remove(item);
        }

        public int Count => _tiles.Count;
        public bool IsReadOnly => true;
        public int IndexOf(ITile item)
        {
            return _tiles.IndexOf(item);
        }

        public void Insert(int index, ITile item)
        {
            _tiles.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _tiles.RemoveAt(index);
        }

        public ITile this[int index]
        {
            get => _tiles[index];
            set => _tiles[index] = value;
        }

        public void Clean()
        {
            var toRemove = _tiles.Where(t => t.Column > _columns - 1 || t.Row > _rows - 1).ToList();
            foreach (var tile in toRemove)
                _tiles.Remove(tile);
        }
    }
}
