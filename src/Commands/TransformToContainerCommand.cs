﻿#region License & Terms
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
using AppTiles.Tiles;
using System.Windows;

namespace AppTiles.Commands
{
    public class TransformToContainerCommand : RefreshableCommand
    {
        public override bool CanExecute(object parameter)
        {
            Logging.DebugLogger.WriteLine($"{GetType().Name}.{nameof(CanExecute)}({parameter})");
            if (parameter is ContainerTile)
                return false;
            return true;
        }

        public override void Execute(object parameter)
        {
            Logging.DebugLogger.WriteLine($"{GetType().Name}.{nameof(Execute)}({parameter})");
            var result = MessageBox.Show("Are you sure you want to transform this tile?", "Warning!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;
            if (!(parameter is ITile tile))
            {
                MessageBox.Show($"Could not transform to {nameof(ContainerTile)}, parameter was of type {parameter.GetType()}.");
                return;
            }

            if (tile.Button == null)
            {
                MessageBox.Show($"Could not transform to {nameof(ContainerTile)}, not connected to a button yet.");
                return;
            }

            var newTile = new ContainerTile(tile.Id, tile.Column, tile.Row, tile.Text, tile.Background, tile.Foreground,
                new TileCollection());
            tile.Button.Update(newTile);
            Settings.ReplaceTile(tile, newTile);
            new EditTileCommand().Execute(newTile);
        }
    }
}
