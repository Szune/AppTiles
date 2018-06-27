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
using System.Diagnostics;

namespace AppTiles.Helpers
{
    public static class ProcessHelper
    {
        public static void Start(string path, string arguments)
        {
            var resolver = new PathResolver(path);
            if (resolver.Type != PathType.File)
                throw new InvalidOperationException("Only file paths support specifying arguments.");
            OpenFile(resolver.Path, arguments);
        }

        public static void Start(string path)
        {
            var resolver = new PathResolver(path);

            Start(resolver);
        }

        public static void Start(PathResolver path)
        {
            switch (path.Type)
            {
                case PathType.Unresolved:
                    throw new InvalidOperationException($"Could not resolve path '{path.OriginalPath}', try specifying the protocol (e.g. 'http://').");
                case PathType.File:
                case PathType.Directory:
                    OpenFile(path.Path);
                    break;
                case PathType.Web:
                case PathType.UnknownProtocol:
                    OpenUsingExplorer(path.Path);
                    break;
                case PathType.ActionVariable:
                    PathVariables.TryExecute(path.Path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(path), path, $"'{nameof(path)}' resolved to unknown {nameof(PathType)}.");
            }
        }


        private static void OpenFile(string path, string arguments = "")
        {
            Process.Start(new ProcessStartInfo { FileName = Uri.UnescapeDataString(path), Arguments = arguments});
        }

        private static void OpenUsingExplorer(string path)
        {
            Process.Start(new ProcessStartInfo { Arguments = SurroundWithQuotes(path), FileName = "explorer.exe" });
        }

        private static string SurroundWithQuotes(string path)
        {
            return $"\"{path.Trim('"')}\"";
        }
    }
}
