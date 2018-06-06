using System;
using System.Windows.Input;
using AppTiles.Tiles;

namespace AppTiles.Commands
{
    public class OpenFolderCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (!(parameter is AppTile app)) return false;
            return app.CanOpenFolder();
        }

        public void Execute(object parameter)
        {
            ((AppTile)parameter).OpenFolder();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
