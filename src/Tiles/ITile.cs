using System.Windows.Controls;
using System.Windows.Media;
using AppTiles.Controls;
using Newtonsoft.Json;

namespace AppTiles.Tiles
{
    public interface ITile
    {
        int Id { get; }
        string Text { get; }
        int Column { get; }
        int Row { get; }
        void Execute();
        Color Background { get; }
        Color Foreground { get; }
        [JsonIgnore]
        TileButton Button { get; }
        void SetButton(TileButton button);
        void Reset();
    }
}
