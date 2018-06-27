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

namespace AppTiles.Helpers
{
    public class PathResolver
    {
        public readonly string OriginalPath;
        public string Path {get; private set;}
        public PathType Type { get; private set; }

        public PathResolver(string originalPath)
        {
            OriginalPath = originalPath;
            SetPathType();
        }

        private void SetPathType()
        {
            if (string.IsNullOrWhiteSpace(OriginalPath))
            {
                Type = PathType.Unresolved;
                Path = "";
                return;
            }

            if (PathVariables.ContainsAction(OriginalPath))
            {
                Path = OriginalPath;
                Type = PathType.ActionVariable;
                return;
            }

            var parsedPath = PathVariables.GetReplacedPath(OriginalPath);
            if (!Uri.TryCreate(parsedPath, UriKind.Absolute, out var pathUri))
            {
                TryResolvePath(parsedPath);
                return;
            }

            if (!pathUri.IsFile) // try to open using explorer.exe
            {
                Type = PathType.UnknownProtocol;
                Path = pathUri.AbsoluteUri;
            }
            else
            {
                Type = PathType.File;
                Path = pathUri.LocalPath;
            }
        }

        private void TryResolvePath(string path)
        {
            // relative path to file?
            if (System.IO.File.Exists(path))
            {
                Type = PathType.File;
                var fileInfo = new System.IO.FileInfo(path);
                Path = fileInfo.FullName;
                return;
            }
            // relative path to folder?
            if (System.IO.Directory.Exists(path))
            {
                Type = PathType.Directory;
                var fileInfo = new System.IO.FileInfo(path);
                Path = fileInfo.FullName;
                return;
            }
            // web url without protocol?
            if (path.StartsWith("www.")) // assume https
            {
                Type = PathType.Web;
                Path = "https://" + path;
                return;
            }

            // unresolved
            Type = PathType.Unresolved;
            Path = "";
        }
    }
}
