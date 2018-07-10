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
using System.Text;

namespace AppTiles.Utilities
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<Exception> GetInnerExceptions(this Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            return GetInnerExceptionsIterator(ex);
        }

        private static IEnumerable<Exception> GetInnerExceptionsIterator(Exception ex)
        {
            var inner = ex.InnerException;
            while (inner != null)
            {
                yield return inner;
                inner = inner.InnerException;
            }
        }

        public static string GetFormattedMessage(this Exception ex)
        {
            var exceptions = GetInnerExceptions(ex).ToList();
            var builder = new StringBuilder();

            builder.AppendLine($"{ex.GetType().Name}: \"{ex.Message}\"");
            for(int i = 0; i < exceptions.Count; i++)
            {
                for (int j = 0; j < i + 1; j++)
                    builder.Append("..");
                builder.AppendLine($"{exceptions[i].GetType().Name}: \"{exceptions[i].Message}\"");
            }

            return builder.ToString();
        }
    }
}
