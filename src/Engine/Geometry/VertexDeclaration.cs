// Author:
//       Gabriel Reiser <gabe@reisergames.com>
//
// Copyright (c) 2007-2023 Reiser Games, LLC.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using Reactor.Common;
using Reactor.Graphics;
using Reactor.Platform;

namespace Reactor.Geometry
{
    public class VertexDeclaration
    {
        private readonly VertexElement[] _elements;

        private readonly Dictionary<int, VertexDeclarationAttributeInfo> shaderAttributeInfo =
            new Dictionary<int, VertexDeclarationAttributeInfo>();

        public VertexDeclaration(params VertexElement[] elements)
            : this(GetVertexStride(elements), elements)
        {
        }

        public VertexDeclaration(uint vertexStride, params VertexElement[] elements)
        {
            if (elements == null || elements.Length == 0)
                throw new ArgumentNullException("elements", "Elements cannot be empty");

            var elementArray = (VertexElement[])elements.Clone();
            _elements = elementArray;
            VertexStride = vertexStride;

            // TODO: Is there a faster/better way to generate a
            // unique hashkey for the same vertex layouts?
            {
                var signature = string.Empty;
                foreach (var element in _elements)
                    signature += element;

                var bytes = Encoding.UTF8.GetBytes(signature);
                HashKey = Hash.ComputeHash(bytes);
            }
        }

        /// <summary>
        ///     A hash value which can be used to compare declarations.
        /// </summary>
        internal int HashKey { get; private set; }

        public uint VertexStride { get; }

        internal void Apply(Program program, IntPtr offset)
        {
            VertexDeclarationAttributeInfo attrInfo;
            var shaderHash = program.GetHashCode();
            if (!shaderAttributeInfo.TryGetValue(shaderHash, out attrInfo))
            {
                // Get the vertex attribute info and cache it
                attrInfo = new VertexDeclarationAttributeInfo(16);

                foreach (var ve in _elements)
                {
                    var attributeLocation = program.GetAttribLocation(ve.VertexElementUsage);

                    if (attributeLocation >= 0)
                    {
                        attrInfo.Elements.Add(new VertexDeclarationAttributeInfo.Element
                        {
                            Offset = ve.Offset,
                            AttributeLocation = attributeLocation,
                            NumberOfElements = VertexElement.OpenGLNumberOfElements(ve.VertexElementFormat),
                            VertexAttribPointerType =
                                VertexElement.OpenGLVertexAttribPointerType(ve.VertexElementFormat),
                            Normalized = VertexElement.OpenGLVertexAttribNormalized(ve)
                        });
                        attrInfo.EnabledAttributes[attributeLocation] = true;
                    }
                }

                shaderAttributeInfo.Add(shaderHash, attrInfo);
            }

            // Apply the vertex attribute info
            /*foreach (var element in attrInfo.Elements)
            {
                GPU.EnableVertexAttribArray((uint)element.AttributeLocation);
                GPU.CheckError();
                unsafe
                {
                    IntPtr ptr = (IntPtr)offset.ToInt64() + element.Offset;
                    GPU.VertexAttribPointer((uint)element.AttributeLocation,
                        element.NumberOfElements,
                        element.VertexAttribPointerType,
                        element.Normalized,
                        VertexStride,
                        (void*)ptr);
                }
                
                GPU.CheckError();
            }*/
            //GraphicsDevice.SetVertexAttributeArray(attrInfo.EnabledAttributes);
        }

        private static uint GetVertexStride(VertexElement[] elements)
        {
            var max = 0;
            for (var i = 0; i < elements.Length; i++)
            {
                int start = elements[i].Offset + VertexElement.GetSize(elements[i].VertexElementFormat);
                if (max < start)
                    max = start;
            }

            return (uint)max;
        }

        public VertexElement[] GetVertexElements()
        {
            return (VertexElement[])_elements.Clone();
        }

        internal static VertexDeclaration FromType(Type vertexType)
        {
            if (vertexType == null)
                throw new ArgumentNullException("vertexType", "Cannot be null");

            if (!ReflectionHelpers.IsValueType(vertexType))
                throw new ArgumentException("vertexType", "Must be value type");

            var type = Activator.CreateInstance(vertexType) as IVertexType;
            if (type == null) throw new ArgumentException("vertexData does not inherit IVertexType");

            var vertexDeclaration = type.Declaration;
            if (vertexDeclaration == null) throw new Exception("VertexDeclaration cannot be null");

            return vertexDeclaration;
        }

        /// <summary>
        ///     Vertex attribute information for a particular shader/vertex declaration combination.
        /// </summary>
        internal class VertexDeclarationAttributeInfo
        {
            internal List<Element> Elements;
            internal bool[] EnabledAttributes;

            internal VertexDeclarationAttributeInfo(int maxVertexAttributes)
            {
                EnabledAttributes = new bool[maxVertexAttributes];
                Elements = new List<Element>();
            }

            internal class Element
            {
                public int AttributeLocation;
                public bool Normalized;
                public int NumberOfElements;
                public int Offset;
                public VertexAttribPointerType VertexAttribPointerType;
            }
        }

        
    }
    
    internal enum VertexAttribPointerType
    {
        Byte = 5120, // 0x00001400
        UnsignedByte = 5121, // 0x00001401
        Short = 5122, // 0x00001402
        UnsignedShort = 5123, // 0x00001403
        Int = 5124, // 0x00001404
        UnsignedInt = 5125, // 0x00001405
        Float = 5126, // 0x00001406
        Double = 5130, // 0x0000140A
        HalfFloat = 5131, // 0x0000140B
        Fixed = 5132, // 0x0000140C
        Int64Arb = 5134, // 0x0000140E
        Int64NV = 5134, // 0x0000140E
        UnsignedInt64Arb = 5135, // 0x0000140F
        UnsignedInt64NV = 5135, // 0x0000140F
        UnsignedInt2101010Rev = 33640, // 0x00008368
        UnsignedInt2101010RevExt = 33640, // 0x00008368
        UnsignedInt10f11f11fRev = 35899, // 0x00008C3B
        Int2101010Rev = 36255, // 0x00008D9F
    }
}