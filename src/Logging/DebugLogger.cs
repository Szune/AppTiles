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
using System.Linq;

namespace AppTiles.Logging
{
    public static class DebugLogger
    {
        #if DEBUG
        private static readonly List<string> Lines = new List<string>();
        #endif
        public static void Write(string msg)
        {
            // Only used in debug
#if DEBUG
            if (Lines.Any())
                Lines[Lines.Count - 1] = Lines[Lines.Count - 1] + msg;
            else
                WriteLine(msg);
#endif
        }

        public static void WriteLine(string msg)
        {
            // Only used in debug
#if DEBUG
            Lines.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {msg}");
#endif
        }

        public static void Save()
        {
            // Only used in debug
#if DEBUG
            System.IO.File.WriteAllLines($"debug{DateTime.Now:yyyyMMdd HHmmss}.txt", Lines);
#endif
        }
    }
}
