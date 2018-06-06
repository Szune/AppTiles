using System;
using System.Windows.Media;
using AppTiles.Windows;

namespace AppTiles.Tiles
{
    public class NoteTile : TileBase
    {
        public string Note { get; set; }

        public NoteTile(int id, int column, int row, string text, Color background, Color foreground, string note) : base(id, column, row, text, background, foreground)
        {
            Note = note;
        }

        public override void Execute()
        {
            var noteWindow = new NoteWindow(this);
            noteWindow.Show();
        }

        public override void Reset()
        {
            Note = "";
            base.Reset();
        }
    }
}
