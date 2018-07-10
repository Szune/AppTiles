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
using AppTiles.Tiles;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppTiles.Utilities
{
    public class TileTypesBinder : ISerializationBinder
    {
        private static readonly List<Type> TileTypes = new List<Type>();

        static TileTypesBinder()
        {
            var baseType = typeof(TileBase);
            var implementedTiles = baseType
                .Assembly
                .DefinedTypes
                .Where(definedType => baseType.Namespace == definedType.Namespace
                                      && definedType.IsSubclassOf(baseType));
            TileTypes.AddRange(implementedTiles);
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            var type = TileTypes
                .FirstOrDefault(t => t.Name == typeName
                                     || typeName == $"AppTiles.Tiles.{t.Name}"); // for backwards compatibility
            if(type == null)
                throw new NotSupportedException($"Tile of type '{typeName}' does not exist.");
            return type;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}
