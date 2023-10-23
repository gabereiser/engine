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
using System.Runtime.InteropServices;
using Reactor.Geometry;
using Reactor.Platform;
using Buffer = Reactor.Platform.WGPU.Wrappers.Buffer;

namespace Reactor.Graphics
{
    public class IndexBuffer : IDisposable
    {
        private readonly bool _isDynamic;
        internal Buffer ibo;

        public IndexBuffer(Type type, int indexCount, BufferUsage usage, bool dynamic)
            : this(SizeForType(type), indexCount, usage, dynamic)
        {
        }

        public IndexBuffer(IndexElementSize indexElementSize, int indexCount, BufferUsage usage, bool dynamic)
        {
            IndexElementSize = indexElementSize;
            IndexCount = indexCount;
            BufferUsage = usage;

            _isDynamic = dynamic;

            GenerateIfRequired();
        }

        public IndexBuffer(IndexElementSize indexElementSize, int indexCount, BufferUsage bufferUsage) :
            this(indexElementSize, indexCount, bufferUsage, false)
        {
        }

        public IndexBuffer(Type type, int indexCount, BufferUsage usage) :
            this(SizeForType(type), indexCount, usage, false)
        {
        }

        public BufferUsage BufferUsage { get; }
        public int IndexCount { get; }
        public IndexElementSize IndexElementSize { get; }

        #region IDisposable implementation

        public void Dispose()
        {
            unsafe
            {
                ibo.Dispose();
                GPU.CheckError();
            }
            
        }

        #endregion

        /// <summary>
        ///     Gets the relevant IndexElementSize enum value for the given type.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="type">The type to use for the index buffer</param>
        /// <returns>The IndexElementSize enum value that matches the type</returns>
        private static IndexElementSize SizeForType(Type type)
        {
            switch (Marshal.SizeOf(type))
            {
                case 2:
                    return IndexElementSize.SixteenBits;
                case 4:
                    return IndexElementSize.ThirtyTwoBits;
                default:
                    throw new ArgumentOutOfRangeException(
                        "Index buffers can only be created for types that are sixteen or thirty two bits in length");
            }
        }
        
        public void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data is null");
            if (data.Length < startIndex + elementCount)
                throw new InvalidOperationException(
                    "The array specified in the data parameter is not the correct size for the amount of data requested.");
            if (BufferUsage == BufferUsage.WriteOnly)
                throw new NotSupportedException(
                    "This IndexBuffer was created with a usage type of BufferUsage.WriteOnly. Calling GetData on a resource that was created with BufferUsage.WriteOnly is not supported.");

#if GLES
            // Buffers are write-only on OpenGL ES 1.1 and 2.0.  See the GL_OES_mapbuffer extension for more information.
            // http://www.khronos.org/registry/gles/extensions/OES/OES_mapbuffer.txt
            throw new NotSupportedException("Index buffers are write-only on OpenGL ES platforms");
#endif
#if !GLES
            GetBufferData(offsetInBytes, data, startIndex, elementCount);
#endif
        }

        public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            GetData(0, data, startIndex, elementCount);
        }

        public void GetData<T>(T[] data) where T : struct
        {
            GetData(0, data, 0, data.Length);
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
        {
            SetDataInternal(offsetInBytes, data, startIndex, elementCount, VertexDataOptions.Discard);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            SetDataInternal(0, data, startIndex, elementCount, VertexDataOptions.Discard);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            SetDataInternal(0, data, 0, data.Length, VertexDataOptions.Discard);
        }

        protected void SetDataInternal<T>(int offsetInBytes, T[] data, int startIndex, int elementCount,
            VertexDataOptions options) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data", "data is null");
            if (data.Length < startIndex + elementCount)
                throw new InvalidOperationException(
                    "The array specified in the data parameter is not the correct size for the amount of data requested.");

            BufferData(offsetInBytes, data, startIndex, elementCount, options);
        }

        private unsafe void GetBufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount) where T : struct
        {
            /*GPU.BindBuffer(GLEnum.ElementArrayBuffer, ibo);
            GPU.CheckError();
            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            var ptr = new IntPtr(GPU.MapBuffer(GLEnum.ArrayBuffer, GLEnum.ReadOnly));
            // Pointer to the start of data to read in the index buffer
            var p = new IntPtr(ptr.ToInt64() + offsetInBytes);
            if (typeof(T) == typeof(byte))
            {
                var buffer = data as byte[];
                // If data is already a byte[] we can skip the temporary buffer
                // Copy from the index buffer to the destination array
                Marshal.Copy(p, buffer, 0, buffer.Length);
            }
            else
            {
                // Temporary buffer to store the copied section of data
                var buffer = new byte[elementCount * elementSizeInByte];
                // Copy from the index buffer to the temporary buffer
                Marshal.Copy(p, buffer, 0, buffer.Length);
                // Copy from the temporary buffer to the destination array
                System.Buffer.BlockCopy(buffer, 0, data, startIndex * elementSizeInByte, elementCount * elementSizeInByte);
            }

            GPU.UnmapBuffer(GLEnum.ElementArrayBuffer);
            GPU.CheckError();
            */
        }

        private void BufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount,
            VertexDataOptions options) where T : struct
        {
            GenerateIfRequired();

            /*var elementSizeInByte = Marshal.SizeOf(typeof(T));
            var sizeInBytes = elementSizeInByte * elementCount;
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInByte);
            var bufferSize = IndexCount * (IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4);

            GPU.BindBuffer(GLEnum.ElementArrayBuffer, ibo);
            GPU.CheckError();

            if (options == VertexDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                GPU.BufferData(GLEnum.ElementArrayBuffer,
                    (UIntPtr)bufferSize,
                    IntPtr.Zero,
                    _isDynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
                GPU.CheckError();
            }

            GPU.BufferData(GLEnum.ElementArrayBuffer, (UIntPtr)bufferSize, dataPtr,
                _isDynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
            GPU.CheckError();
            GPU.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            GPU.CheckError();
            dataHandle.Free();
            */
        }

        /// <summary>
        ///     If the IBO does not exist, create it.
        /// </summary>
        private unsafe void GenerateIfRequired()
        {
            if (ibo == null)
            {
                /*var sizeInBytes = IndexCount * (IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4);
                uint[] p = { 0 };
                GPU.GenBuffers(1, p);
                ibo = p[0];
                GPU.CheckError();
                GPU.BindBuffer(GLEnum.ElementArrayBuffer, ibo);
                GPU.CheckError();
                GPU.BufferData(GLEnum.ElementArrayBuffer,
                    (UIntPtr)sizeInBytes, IntPtr.Zero,
                    _isDynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
                GPU.CheckError();
                */
            }
        }

        internal int GetElementCountArray(PrimitiveType primitiveType, int primitiveCount)
        {
            //TODO: Overview the calculation
            switch (primitiveType)
            {
                case PrimitiveType.Points:
                    return primitiveCount;
                case PrimitiveType.Lines:
                    return primitiveCount * 2;
                case PrimitiveType.LineStrip:
                    return primitiveCount + 1;
                case PrimitiveType.Triangles:
                    return primitiveCount * 3;
                case PrimitiveType.TriangleStrip:
                    return 3 + (primitiveCount - 1); // ???
                case PrimitiveType.Polygon:
                    return primitiveCount * 4;
            }

            throw new NotSupportedException();
        }

        internal void Bind()
        {
            //GPU.BindBuffer(GLEnum.ElementArrayBuffer, ibo);
        }

        internal void Unbind()
        {
            //GPU.BindBuffer(GLEnum.ElementArrayBuffer, 0);
        }
    }
}