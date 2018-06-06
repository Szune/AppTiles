using System.Windows;
using System.Windows.Input;
using AppTiles.Helpers;

namespace AppTiles.Windows
{
    /// <summary>
    /// Interaction logic for ArgumentInputWindow.xaml
    /// </summary>
    public partial class ArgumentInputWindow : Window
    {
        private readonly string _path;
        private readonly string _arguments;

        public ArgumentInputWindow(string path, string parentText, string arguments)
        {
            _path = path;
            _arguments = arguments;
            InitializeComponent();
            TxtArguments.Focus();
            Title = "Input - " + parentText;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Start();
            }
            else if (e.Key == Key.Escape)
                Close();
        }

        private void Start()
        {
            if(!string.IsNullOrWhiteSpace(_arguments))
                ProcessHelper.StartProcess(_path, _arguments + " " + TxtArguments.Text);
            else
                ProcessHelper.StartProcess(_path, TxtArguments.Text);
            Close();
        }
    }
}
