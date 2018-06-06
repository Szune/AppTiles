using System;
using System.Windows.Input;
using AppTiles.Tiles;

namespace AppTiles.Commands
{
    public class ExecuteTileCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ((ITile)parameter).Execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
