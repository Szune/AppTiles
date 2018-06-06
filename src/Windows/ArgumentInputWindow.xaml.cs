﻿#region License & Terms
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