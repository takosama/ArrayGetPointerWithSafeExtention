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
