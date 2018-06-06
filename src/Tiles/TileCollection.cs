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
            _columns = Settings.DefaultColumnAmount;
            _rows = Settings.DefaultRowAmount;
            _width = Settings.DefaultWidth;
            _height = Settings.DefaultHeight;
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
