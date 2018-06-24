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
using AppTiles.Windows;
using System;
using System.Collections.Generic;

namespace AppTiles.Helpers
{
    public static class PathVariables
    {
        private static readonly Dictionary<string, Func<string>> Replacers = new Dictionary<string, Func<string>>();
        private static readonly Dictionary<string, Action> Actions = new Dictionary<string, Action>();

        private static Settings _settings;

        static PathVariables()
        {
            Replacers.Add("{AppFolder}", () => Environment.CurrentDirectory);
            Actions.Add("{AppSettings}", () =>
            {
                new SettingsWindow(_settings).ShowDialog();
            });
        }

        public static string GetReplacedPath(string path)
        {
            var truePath = path;
            foreach (var variable in Replacers)
            {
                if (truePath.Contains(variable.Key))
                {
                    truePath = truePath.Replace(variable.Key, variable.Value());
                }
            }

            return truePath;
        }

        public static bool TryExecute(string path)
        {
            foreach (var variable in Actions)
            {
                if (path.Contains(variable.Key))
                {
                    variable.Value();
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsAction(string path)
        {
            foreach (var variable in Actions)
            {
                if (path.Contains(variable.Key))
                {
                    return true;
                }
            }

            return false;
        }

        public static void UseSettings(Settings settings)
        {
            _settings = settings;
        }
    }
}