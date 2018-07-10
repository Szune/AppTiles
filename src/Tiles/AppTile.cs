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
using AppTiles.Windows;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Media;
using AppTiles.Utilities;

namespace AppTiles.Tiles
{
    public class AppTile : TileBase
    {
        private string _path;

        [ShowInEditor]
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                RefreshFolderAvailability();
            }
        }
        [ShowInEditor(DisplayText = "Is taking input", IsAdvanced = true)]
        public bool IsTakingInput { get; set; }
        [ShowInEditor]
        public string Arguments { get; set; }

        private bool _canOpenFolder;

        public AppTile(int id, int column, int row) : this(id, column, row, "-", Colors.Black, Colors.White, "", "")
        {
        }

        [JsonConstructor]
        public AppTile(int id, int column, int row, string text, Color background, Color foreground, string path,
            string arguments, bool isTakingInput = false) : base(id, column, row, text, background, foreground)
        {
            Path = path;
            Arguments = arguments;
            IsTakingInput = isTakingInput;
        }

        private void RefreshFolderAvailability()
        {
            Button?.RefreshCommands();
            if (string.IsNullOrWhiteSpace(Path))
                _canOpenFolder = false; // exit asap
            var resolver = new PathResolver(Path);
            _canOpenFolder = resolver.Type == PathType.File || resolver.Type == PathType.Directory;
        }

        public bool GetFolderAvailability()
        {
            return _canOpenFolder;
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                MessageBox.Show("No path set.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsTakingInput)
            {
                new ArgumentInputWindow(Path, Text, Arguments).ShowDialog();
                return;
            }

            try
            {
                if(string.IsNullOrWhiteSpace(Arguments))
                    ProcessHelper.Start(Path);
                else
                    ProcessHelper.Start(Path, Arguments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open app:{Environment.NewLine}{ex.GetFormattedMessage()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Reset()
        {
            Path = "";
            Arguments = "";
            IsTakingInput = false;
            base.Reset();
        }

        public void OpenFolder()
        {
            try
            {
                var resolver = new PathResolver(Path);
                var folder = System.IO.Path.GetDirectoryName(resolver.Path);
                ProcessHelper.Start(folder);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Could not open folder:{Environment.NewLine}{ex.GetFormattedMessage()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
