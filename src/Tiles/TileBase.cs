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
        }

        public virtual void Reset()
        {
            Text = "-";
            Background = Colors.Black;
            Foreground = Colors.White;
        }
    }
}
