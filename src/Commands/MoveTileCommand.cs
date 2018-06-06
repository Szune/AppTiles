using System;
using System.Windows;
using System.Windows.Input;
using AppTiles.Tiles;

namespace AppTiles.Commands
{
    public class MoveTileCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MoveTile((ITile) parameter);
        }

        private static void MoveTile(ITile tile)
        {
            DragDrop.DoDragDrop(tile.Button, new DataObject(typeof(ITile), tile), DragDropEffects.Move);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
