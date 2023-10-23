using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Reactor.Common
{
    /// <summary>
    ///     Allocator has some pre-defined type arrays useful for Interop with underlying platform memory.
    ///     Instead of creating new types, we reuse memory and thus increase speed by reducing allocations.
    /// </summary>
    public static class Allocator
    {
        private static int _allocations;
        private static List<Memory<byte>> _fences = new List<Memory<byte>>();
        
        public static int Allocations
        {
            get
            {
                return _allocations;
            }
        }

        public static ulong MemoryAllocated
        {
            get
            {
                ulong l = 0UL;
                
                foreach (var f in _fences)
                {
                    l += (ulong)f.Length;
                }
                return l;
            }
        }
        
        public static Memory<byte> Fence(int index)
        {
            return _fences[index];
        }

        public static int CreateFence(ulong size)
        {
            unsafe
            {
                var fence = new Memory<byte>((byte[])Marshal.PtrToStructure(new IntPtr(Malloc(size)), typeof(byte[])), 0, (int)size);
                _fences.Add(fence);
                return _fences.Count - 1;
            }
        }

        public static void DestroyFence(int index)
        {
            unsafe
            {
                Free(_fences[index].Pin().Pointer);
            }
            _fences.RemoveAt(index);
        }

        internal static void Allocated()
        {
            Interlocked.Increment(ref _allocations);
        }

        internal static void Freed()
        {
            Interlocked.Decrement(ref _allocations);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* Malloc(ulong size)
        {
            return Malloc((long)size);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* Malloc(long size)
        {
            var ptr = Marshal.AllocHGlobal((int)size);

            Allocated();

            return ptr.ToPointer();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Free(void* a)
        {
            if (a == null)
                return;

            var ptr = new IntPtr(a);
            Marshal.FreeHGlobal(ptr);
            Freed();
        }
    }
}