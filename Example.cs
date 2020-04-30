using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.InteropServices;
using System.Linq;
using ArrayGetPointerWithSafe;

namespace ConsoleApp20
{
    class Program
    {
        unsafe static void Main(string[] args)
        {


            float[] array = Enumerable.Range(1, 100).Select(x => (float)x).ToArray();

            //自分で開放
            var arrayPtro = new ArrayGetPointerWithSafe<float>(array);
            float* arrayPtr = arrayPtro.ptr;
            for (int i = 0; i < array.Length; i++)
                Console.WriteLine(*(arrayPtr + i));
            arrayPtro.Dispose();

            //自動解放
            array.GetPointerObject(tmp =>
            {
                float* pa = tmp.ptr;
                for (int i = 0; i < tmp.Length; i++)
                    Console.WriteLine(*(pa + i));
            });
        }
    }
}

namespace ArrayGetPointerWithSafe
{
    public static class ArrayGetPointerWithSafeExtention
    {
        public static void GetPointerObject<T>(this T[] array, Action<ArrayGetPointerWithSafe<T>> action) where T : unmanaged
        {
            var _ = new ArrayGetPointerWithSafe<T>(array);
            action(_);
            _.Dispose();
        }
    }

    unsafe public class ArrayGetPointerWithSafe<T> where T : unmanaged
    {
        public T* ptr { get; }
        public int Length { get; }
        GCHandle gch;
        public bool IsDisposed { get; private set; } = false;
        public ArrayGetPointerWithSafe(T[] array)
        {
            gch = GCHandle.Alloc(array, GCHandleType.Pinned);
            this.ptr = (T*)gch.AddrOfPinnedObject().ToPointer();
            this.Length = array.Length;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                this.gch.Free();
                IsDisposed = true;
            }
        }
    }
}
