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
using Red.Geometry;
using Red.Platform;
using Buffer = Red.Platform.WGPU.Wrappers.Buffer;


namespace Red.Graphics
{
    public class VertexBuffer : IDisposable
    {
        internal bool _isDynamic;
        internal bool IsDisposed;

        public VertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage,
            bool dynamic)
        {
            if (vertexDeclaration == null)
                throw new ArgumentNullException("vertexDeclaration", "vertexDeclaration not set! was null.");
            VertexDeclaration = vertexDeclaration;
            VertexCount = vertexCount;
            BufferUsage = bufferUsage;

            _isDynamic = dynamic;

            GenerateIfRequired();
        }

        public VertexBuffer(Type type, int vertexCount, BufferUsage bufferUsage, bool dynamic) :
            this(VertexDeclaration.FromType(type), vertexCount, bufferUsage, dynamic)
        {
        }

        public VertexBuffer(VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            this(vertexDeclaration, vertexCount, bufferUsage, false)
        {
        }

        public VertexBuffer(Type type, int vertexCount, BufferUsage bufferUsage) :
            this(VertexDeclaration.FromType(type), vertexCount, bufferUsage, false)
        {
        }

        public int VertexCount { get; }
        public VertexDeclaration VertexDeclaration { get; }
        public BufferUsage BufferUsage { get; }

        public unsafe Buffer* VBO { get; private set; }


        public void Dispose()
        {
            if (!IsDisposed)
            {
                //GPU.DeleteBuffer(VBO);
                GPU.CheckError();
            }
            IsDisposed = true;
        }

        public void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
            where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data", "This method does not accept null for this parameter.");
            if (data.Length < startIndex + elementCount)
                throw new ArgumentOutOfRangeException("elementCount",
                    "This parameter must be a valid index within the array.");
            if (BufferUsage == BufferUsage.WriteOnly)
                throw new NotSupportedException(
                    "Calling GetData on a resource that was created with BufferUsage.WriteOnly is not supported.");
            if (elementCount * vertexStride > VertexCount * VertexDeclaration.VertexStride)
                throw new InvalidOperationException(
                    "The array is not the correct size for the amount of data requested.");

            GetBufferData(offsetInBytes, data, startIndex, elementCount, vertexStride);
        }

        public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            GetData(0, data, startIndex, elementCount, elementSizeInByte);
        }

        public void GetData<T>(T[] data) where T : struct
        {
            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            GetData(0, data, 0, data.Length, elementSizeInByte);
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
            where T : struct
        {
            SetDataInternal(offsetInBytes, data, startIndex, elementCount, (int)VertexDeclaration.VertexStride,
                VertexDataOptions.None);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            SetDataInternal(0, data, startIndex, elementCount, (int)VertexDeclaration.VertexStride, VertexDataOptions.None);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            SetDataInternal(0, data, 0, data.Length, (int)VertexDeclaration.VertexStride, VertexDataOptions.None);
        }

        protected void SetDataInternal<T>(int offsetInBytes, T[] data, int startIndex, int elementCount,
            int vertexStride, VertexDataOptions options) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data is null");
            if (data.Length < startIndex + elementCount)
                throw new InvalidOperationException(
                    "The array specified in the data parameter is not the correct size for the amount of data requested.");

            var bufferSize = VertexCount * VertexDeclaration.VertexStride;

            if (vertexStride > bufferSize || vertexStride < VertexDeclaration.VertexStride)
                throw new ArgumentOutOfRangeException(
                    "One of the following conditions is true:\nThe vertex stride is larger than the vertex buffer.\nThe vertex stride is too small for the type of data requested.");

            var elementSizeInBytes = Marshal.SizeOf(typeof(T));

            SetBufferData((int)bufferSize, elementSizeInBytes, offsetInBytes, data, startIndex, elementCount,
                    vertexStride, options);
        }

        /// <summary>
        ///     If the VBO does not exist, create it.
        /// </summary>
        private unsafe void GenerateIfRequired()
        {
            if (VBO == null)
            {
                /*VAO = GPU.GenVertexArray();
                GPU.CheckError();
                GPU.BindVertexArray(VAO);
                GPU.CheckError();

                VBO = GPU.GenBuffer();
                GPU.CheckError();
                GPU.BindBuffer(GLEnum.ArrayBuffer, VBO);
                GPU.CheckError();
                GPU.BufferData(GLEnum.ArrayBuffer,
                    new UIntPtr((ulong)(VertexDeclaration.VertexStride * VertexCount)), IntPtr.Zero,
                    _isDynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
                GPU.CheckError();
                */
            }
        }


        private unsafe void GetBufferData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
            where T : struct
        {
            /*GPU.BindBuffer(GLEnum.ArrayBuffer, VBO);
            GPU.CheckError();
            var elementSizeInByte = Marshal.SizeOf(typeof(T));
            var ptr = new IntPtr(GPU.MapBuffer(GLEnum.ArrayBuffer, GLEnum.ReadOnly));
            GPU.CheckError();
            // Pointer to the start of data to read in the index buffer
            ptr = new IntPtr(ptr.ToInt64() + offsetInBytes);
            if (typeof(T) == typeof(byte))
            {
                var buffer = data as byte[];
                // If data is already a byte[] we can skip the temporary buffer
                // Copy from the vertex buffer to the destination array
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
            }
            else
            {
                // Temporary buffer to store the copied section of data
                var buffer = new byte[elementCount * vertexStride - offsetInBytes];
                // Copy from the vertex buffer to the temporary buffer
                Marshal.Copy(ptr, buffer, 0, buffer.Length);

                var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInByte);

                // Copy from the temporary buffer to the destination array

                var dataSize = Marshal.SizeOf(typeof(T));
                if (dataSize == vertexStride)
                    Marshal.Copy(buffer, 0, dataPtr, buffer.Length);
                else
                    // If the user is asking for a specific element within the vertex buffer, copy them one by one...
                    for (var i = 0; i < elementCount; i++)
                    {
                        Marshal.Copy(buffer, i * vertexStride, dataPtr, dataSize);
                        dataPtr = (IntPtr)(dataPtr.ToInt64() + dataSize);
                    }

                dataHandle.Free();

                //Buffer.BlockCopy(buffer, 0, data, startIndex * elementSizeInByte, elementCount * elementSizeInByte);
            }

            GPU.UnmapBuffer(GLEnum.ArrayBuffer);
            GPU.CheckError();
            */
        }


        private void SetBufferData<T>(int bufferSize, int elementSizeInBytes, int offsetInBytes, T[] data,
            int startIndex, int elementCount, int vertexStride, VertexDataOptions options) where T : struct
        {
            GenerateIfRequired();

            /*var sizeInBytes = elementSizeInBytes * elementCount;
            GPU.Gl.BindBuffer(GLEnum.ArrayBuffer, VBO);
            GPU.CheckError();

            if (options == VertexDataOptions.Discard)
            {
                // By assigning NULL data to the buffer this gives a hint
                // to the device to discard the previous content.
                GPU.Gl.BufferData(GLEnum.ArrayBuffer,
                    (uint)bufferSize,
                    in IntPtr.Zero,
                    _isDynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
                GPU.CheckError();
            }

            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInBytes);

            //GPU.BufferSubData(GLEnum.ArrayBuffer, (IntPtr)offsetInBytes, (UIntPtr)(uint)sizeInBytes, dataPtr);
            //GPU.CheckError();

            dataHandle.Free();
            */
        }

        public void Bind()
        {
            //GPU.BindBuffer(GLEnum.ArrayBuffer, VBO);
            //GPU.CheckError();
        }

        public void Unbind()
        {
            //GPU.BindBuffer(GLEnum.ArrayBuffer, 0);
            //GPU.CheckError();
        }

        public void BindVertexArray()
        {
            GenerateIfRequired();
            //GPU.CheckError();
            //GPU.BindVertexArray(VAO);
            //GPU.CheckError();
        }

        public void UnbindVertexArray()
        {
            //GPU.BindVertexArray(0);
            //GPU.CheckError();
        }

        public void Clear()
        {
        }
    }
}