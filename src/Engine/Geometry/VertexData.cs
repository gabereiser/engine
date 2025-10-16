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


using Red.Math3D;

namespace Red.Geometry
{
    public struct VertexData : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Bitangent;
        public Vector3 Tangent;
        public Vector2 TexCoord;

        private static readonly VertexDeclaration VertexDeclaration;

        public VertexDeclaration Declaration => VertexDeclaration;

        public VertexData(Vector3 Position, Vector3 Normal, Vector3 Bitangent, Vector3 Tangent, Vector2 TexCoord)
        {
            this.Position = Position;
            this.Normal = Normal;
            this.TexCoord = TexCoord;
            this.Bitangent = Bitangent;
            this.Tangent = Tangent;
        }

        static VertexData()
        {
            VertexElement[] elements =
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position),
                new VertexElement(sizeof(float) * 3 * 1, VertexElementFormat.Vector3, VertexElementUsage.Normal),
                new VertexElement(sizeof(float) * 3 * 2, VertexElementFormat.Vector3, VertexElementUsage.Bitangent),
                new VertexElement(sizeof(float) * 3 * 3, VertexElementFormat.Vector3, VertexElementUsage.Tangent),
                new VertexElement(sizeof(float) * 3 * 4, VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate)
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }

    public struct VertexData2D : IVertexType
    {
        public Vector2 Position;
        public Vector2 TexCoord;


        private static readonly VertexDeclaration VertexDeclaration;

        public VertexDeclaration Declaration => VertexDeclaration;

        public VertexData2D(Vector2 Position, Vector2 TexCoord)
        {
            this.Position = Position;
            this.TexCoord = TexCoord;
        }


        static VertexData2D()
        {
            VertexElement[] elements =
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position),
                new VertexElement(sizeof(float) * 2 * 1, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate)
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }
    }
}