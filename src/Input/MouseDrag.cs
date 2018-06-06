using AppTiles.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AppTiles.Input
{
    public static class MouseDrag
    {
        private static Point _mouseDownPosition = new Point(-1, -1);
        private static TileButton _sender;
        public static TileButton LastSender => _sender;

        public static bool IsBeingPerformed
        {
            get
            {
                if (_mouseDownPosition.X == -1 && _mouseDownPosition.Y == -1) // not dragging
                    return false;
                return true;
            }
        }

        public static bool HasMoved(Point pos)
        {
            if ((Math.Abs(pos.X - _mouseDownPosition.X) <= 0) ||
                (Math.Abs(pos.Y - _mouseDownPosition.Y) <= 0) ||
                _sender == null)
                return false;
            return true;
        }

        public static void ResetPosition()
        {
            _mouseDownPosition = new Point(-1, -1);
        }

        public static void ResetAll()
        {
            ResetPosition();
            _sender = null;
        }

        public static void StartNew(TileButton sender, Point pos)
        {
            _sender = sender;
            _mouseDownPosition = pos;
        }
    }
}
