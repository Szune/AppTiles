using AppTiles.Attributes;
using AppTiles.Helpers;
using AppTiles.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppTiles.Windows
{
    /// <summary>
    /// Interaction logic for EditTileWindow.xaml
    /// </summary>
    public partial class EditTileWindow : Window
    {
        private ITile Tile { get; set; }


        private readonly Dictionary<PropertyInfo, UIElement> _editableControls = new Dictionary<PropertyInfo, UIElement>();
        private int _generatedHeight;
        private int _collapsedHeight;
        private int _expandedHeight;
        public EditTileWindow(ITile tile)
        {
            Tile = tile;
            InitializeComponent();
            MainGrid.Children.Clear();
            CreateControlsFromProperties(tile);
        }

        private void CreateControlsFromProperties(ITile tile)
        {
            var props = tile.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop =>
                prop.GetCustomAttribute<ShowInEditorAttribute>() != null).ToList();

            (var advancedProps, var regularProps) = props
                .OrderByDescending(p => p.GetCustomAttribute<ShowInEditorAttribute>().IsBaseClass)
                .Separate(p => p.GetCustomAttribute<ShowInEditorAttribute>().IsAdvanced);

            // create stackpanels 
            (var stackPanelLeft, var stackPanelRight) = CreateControlsFromProperties(regularProps, tile);

            // set rows
            Grid.SetRow(stackPanelLeft, 0);
            Grid.SetRow(stackPanelRight, 0);

            var saveButton = new Button
            {
                Height = ControlCreator.ControlHeight,
                Content = "Save"
            };
            saveButton.Click += SaveButton_Click;
            stackPanelRight.Children.Add(saveButton);
            _generatedHeight += ControlCreator.ControlHeight;

            MainGrid.Children.AddRange(new [] {stackPanelLeft, stackPanelRight});
            // add first row definition
            MainGrid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = new GridLength(_generatedHeight)
                });

            // reset height
            _collapsedHeight = _generatedHeight + 50;
            _generatedHeight = 0;

            // show advanced properties if any
            if (!advancedProps.Any())
            {
                ForceHeight(_collapsedHeight);
                return;
            }

            // initialize expander grid
            var expanderGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(0.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)}
                }
            };

            (var advancedStackPanelLeft, var advancedStackPanelRight) =
                CreateControlsFromProperties(advancedProps, tile);

            expanderGrid.Children.AddRange(new[] { advancedStackPanelLeft, advancedStackPanelRight});

            // add expander
            var advancedExpander = new Expander
            {
                Header = "Advanced",
                Content = expanderGrid
            };
            Grid.SetColumn(advancedExpander, 0);
            Grid.SetColumnSpan(advancedExpander, 2);
            Grid.SetRow(advancedExpander, 1);
            const int expanderHeight = 30;
            _generatedHeight += expanderHeight; // add expander size
            _collapsedHeight += expanderHeight;
            MainGrid.Children.Add(advancedExpander);
            // add second row definition
            MainGrid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height= new GridLength(_generatedHeight)
                });
            _expandedHeight = _collapsedHeight + _generatedHeight - expanderHeight;
            // force collapsed height to begin with
            ForceHeight(_collapsedHeight);
            // add events for collapsing/expanding
            advancedExpander.Collapsed += (obj, args) =>
            {
                ForceHeight(_collapsedHeight);
            };
            advancedExpander.Expanded += (obj, args) =>
            {
                ForceHeight(_expandedHeight);
            };
        }

        private void ForceHeight(int height)
        {
            Height = height;
            MinHeight = height;
            MaxHeight = height;
        }

        private (StackPanel left, StackPanel right) CreateControlsFromProperties(List<PropertyInfo> properties, ITile tile)
        {
            var stackPanelLeft = new StackPanel { Orientation = Orientation.Vertical };
            var stackPanelRight = new StackPanel { Orientation = Orientation.Vertical };

            // show regular properties
            foreach (var prop in properties)
            {
                (var leftSideControl, var rightSideControl) = EditorHelper.GetControlRowFromProperty(prop, tile);
                stackPanelLeft.Children.Add(leftSideControl);
                _editableControls[prop] = rightSideControl; // have the controls in the same place to make it easier when assigning to props
                stackPanelRight.Children.Add(rightSideControl);
                _generatedHeight += ControlCreator.ControlHeight;
            }
            Grid.SetColumn(stackPanelLeft, 0);
            Grid.SetColumn(stackPanelRight, 1);

            return (stackPanelLeft, stackPanelRight);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            try
            {
                foreach (var kvp in _editableControls)
                {
                    EditorHelper.SetPropertyValueFromControl(kvp.Key, kvp.Value, Tile);
                }

                Settings.SetChanged();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save:{Environment.NewLine}\"{ex.Message}\"", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

       
        private void EditTileWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Save();
            }
            else if (e.Key == Key.Escape)
                Close();
        }



        public void SetValuesFromProperties(ITile tile)
        {
            Tile = tile;
            foreach (var item in _editableControls)
            {
                EditorHelper.SetControlValueFromProperty(item.Value, item.Key, tile);
            }
        }

        private void EditTileWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
