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
using AppTiles.Utilities;

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

            var properties = props
                .OrderByDescending(p => p.GetCustomAttribute<ShowInEditorAttribute>().IsBaseClass)
                .Separate(p => p.GetCustomAttribute<ShowInEditorAttribute>().IsAdvanced);
            var regularProperties = properties.FalseList;
            var advancedProperties = properties.TrueList;

            // create stackpanels 
            var rows = CreateControlsFromProperties(regularProperties, tile);

            // set rows
            Grid.SetRow(rows.Left, 0);
            Grid.SetRow(rows.Right, 0);

            var saveButton = new Button
            {
                Height = ControlCreator.ControlHeight,
                Content = "Save"
            };
            saveButton.Click += SaveButton_Click;
            // add save button to right side
            rows.Right.Children.Add(saveButton);
            _generatedHeight += ControlCreator.ControlHeight;

            MainGrid.Children.AddRange(new [] {rows.Left, rows.Right});
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
            if (!advancedProperties.Any())
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

            var advancedRows = CreateControlsFromProperties(advancedProperties, tile);

            expanderGrid.Children.AddRange(new[] { advancedRows.Left, advancedRows.Right});

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

        private ControlRow<StackPanel, StackPanel> CreateControlsFromProperties(List<PropertyInfo> properties, ITile tile)
        {
            var stackPanelLeft = new StackPanel { Orientation = Orientation.Vertical };
            var stackPanelRight = new StackPanel { Orientation = Orientation.Vertical };

            // show regular properties
            foreach (var prop in properties)
            {
                var row = EditorHelper.GetControlRowFromProperty(prop, tile);
                stackPanelLeft.Children.Add(row.Left);
                _editableControls[prop] = row.Right; // have the controls in the same place to make it easier when assigning to props
                stackPanelRight.Children.Add(row.Right);
                _generatedHeight += ControlCreator.ControlHeight;
            }
            Grid.SetColumn(stackPanelLeft, 0);
            Grid.SetColumn(stackPanelRight, 1);

            return new ControlRow<StackPanel, StackPanel>(stackPanelLeft, stackPanelRight);
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
