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
using AppTiles.Controls;
using System;
using System.Windows;

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
