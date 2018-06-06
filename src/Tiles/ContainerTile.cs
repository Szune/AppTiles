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
