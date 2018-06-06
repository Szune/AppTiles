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

namespace AppTiles.Tiles
{
    public class AppTile : TileBase
    {
        [ShowInEditor]
        public string Path { get; set; }
        [ShowInEditor(DisplayText = "Is taking input", IsAdvanced = true)]
        public bool IsTakingInput { get; set; }
        [ShowInEditor]
        public string Arguments { get; set; }


        public AppTile(int id, int column, int row) : base(id, column, row, "-", Colors.Black, Colors.White)
        {
            Path = "";
            Arguments = "";
            IsTakingInput = false;
        }

        [JsonConstructor]
        public AppTile(int id, int column, int row, string text, Color background, Color foreground, string path,
            string arguments, bool isTakingInput = false) : base(id, column, row, text, background, foreground)
        {
            Path = path;
            Arguments = arguments;
            IsTakingInput = isTakingInput;
        }

        public override void Execute()
        {
            var path = Path;
            if (string.IsNullOrWhiteSpace(path))
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
                path = ProcessHelper.ReplaceVariables(path);
                if(string.IsNullOrWhiteSpace(Arguments))
                    ProcessHelper.StartProcess(path);
                else
                    ProcessHelper.StartProcess(path, Arguments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open app:{Environment.NewLine}\"{ex.Message}\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Reset()
        {
            Path = "";
            Arguments = "";
            IsTakingInput = false;
            base.Reset();
        }

        public bool CanOpenFolder()
        {
            if (string.IsNullOrWhiteSpace(Path))
                return false; // exit asap
            var path = ProcessHelper.ReplaceVariables(Path);
            if (!string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(path)))
                path = System.IO.Path.GetDirectoryName(path);

            return !ProcessHelper.HasProtocol(path);
            // I could use Directory.Exists(), but considering the amount of times that CanExecute() can be checked on a command
            // I'd rather do file system checks fewer times and have a 'good enough' solution here
            // will be improved upon later
        }

        public void OpenFolder()
        {
            try
            {
                var path = ProcessHelper.ReplaceVariables(Path);
                if (!string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(path)))
                    path = System.IO.Path.GetDirectoryName(path);

                ProcessHelper.StartProcess(path);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Could not open folder:{Environment.NewLine}\"{ex.Message}\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
