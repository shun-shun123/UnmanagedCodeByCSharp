using System;
using System.Runtime.CompilerServices;

namespace UnmanagedSample
{
    unsafe class Program
    {
        static ulong AsUnmanaged<T>(T reference) where T : class => (ulong) Unsafe.As<T, IntPtr>(ref reference);

        static ulong AsUnmanaged<T>(ref T reference) => (ulong) Unsafe.AsPointer(ref reference);
        
        static void Main(string[] args)
        {
            // 意図的にGCゴミを作成
            void GenerateGarbage()
            {
                for (var i = 0; i < 1000000; i++)
                {
                    var dummy = new object();
                }
            }

            GenerateGarbage();
            
            var data = new Data {Value = 1234};
            ref var reference = ref data.Value;

            var addressOfData = AsUnmanaged(data);
            var addressOfValue = AsUnmanaged(ref reference);
            
            Console.WriteLine((addressOfData, addressOfValue));
            
            GenerateGarbage();
            GC.Collect(0, GCCollectionMode.Forced);
            Console.WriteLine("-----ここでGC発生-----");
            
            Console.WriteLine("unmanaged: " + (addressOfData, addressOfValue));
            
            // GC発生後はコンパンクションなどによってアドレスが変わる
            Console.WriteLine("managed: " + (AsUnmanaged(data), AsUnmanaged(ref reference)));
            
            fixed (int* p = &data.Value)
            {
                GenerateGarbage();
                GC.Collect(0, GCCollectionMode.Forced);
                Console.WriteLine("-----ここでGC発生(fixed)-----");
                Console.WriteLine("managed: " + (AsUnmanaged(data), AsUnmanaged(ref reference)));
            }
        }
    }

    class Data
    {
        public int Value;
    }
}
