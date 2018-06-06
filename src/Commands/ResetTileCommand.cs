using AppTiles.Tiles;
using System;
using System.Windows;
using System.Windows.Input;

namespace AppTiles.Commands
{
    public class ResetTileCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var result = MessageBox.Show("Are you sure you want to reset this tile?", "Warning!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;
            ((ITile) parameter).Reset();
            Settings.SetChanged();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
