using AppTiles.Tiles;
using System;
using System.Windows.Input;
using AppTiles.Helpers;

namespace AppTiles.Commands
{
    public class EditTileCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            EditorHelper.GetEditTileWindow((ITile) parameter).ShowDialog();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
