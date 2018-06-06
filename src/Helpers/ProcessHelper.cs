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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppTiles.Windows;

namespace AppTiles.Helpers
{
    public static class ProcessHelper
    {
        private static readonly Dictionary<string, Func<string>> ReplacedVariables = new Dictionary<string, Func<string>>();
        private static readonly Dictionary<string, Action> ActionVariables = new Dictionary<string, Action>();

        private static Settings _settings;

        static ProcessHelper()
        {
            ReplacedVariables.Add("{AppFolder}", () => Environment.CurrentDirectory);
            ActionVariables.Add("{AppSettings}", () =>
            {
                new SettingsWindow(_settings).ShowDialog();
            });
        }

        public static string ReplaceVariables(string path)
        {
            var truePath = path;
            foreach (var variable in ReplacedVariables)
            {
                if (truePath.Contains(variable.Key))
                {
                    truePath = truePath.Replace(variable.Key, variable.Value());
                }
            }

            return truePath;
        }

        public static void StartProcess(string path)
        {
            if (PerformVariableAction(path))
                return;
            if (string.IsNullOrWhiteSpace(path)) return;
            if (path.StartsWith("http"))
                Process.Start(new ProcessStartInfo {Arguments = SurroundWithQuotes(path), FileName = "explorer.exe"});
            else if (path.StartsWith("file"))
                Process.Start(new ProcessStartInfo {FileName = Uri.UnescapeDataString(path)});
            else
                Process.Start(SurroundWithQuotes(path));
        }

        private static bool PerformVariableAction(string path)
        {
            foreach (var variable in ActionVariables)
            {
                if (!path.Contains(variable.Key))
                    continue;
                variable.Value();
                return true;
            }

            return false;
        }

        public static void StartProcess(string path, string arguments)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(arguments)) return;
            if (HasProtocol(path)) return;
            Process.Start(new ProcessStartInfo {Arguments = SurroundWithQuotes(arguments), FileName = path});
        }

        public static void UseSettings(Settings settings)
        {
            _settings = settings;
        }

        private static string SurroundWithQuotes(string path)
        {
            return $"\"{path.Trim('"')}\"";
        }

        public static bool HasProtocol(string path)
        {
            return path.Contains("://");
        }
    }
}