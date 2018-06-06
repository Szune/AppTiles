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