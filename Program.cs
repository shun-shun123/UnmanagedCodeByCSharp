using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Running;
using LinqBenchmark.Benchmark;
using UnmanagedSample.SpanTutorial;

namespace UnmanagedSample
{
    unsafe class Program
    {
        static ulong AsUnmanaged<T>(T reference) where T : class => (ulong) Unsafe.As<T, IntPtr>(ref reference);

        static ulong AsUnmanaged<T>(ref T reference) => (ulong) Unsafe.AsPointer(ref reference);

        /// <summary>
        /// ManagedとUnmanagedの領域でどのようにメモリアドレスが管理されているか
        /// コンパンクションが起きた後どのような値になっているか
        /// fixedの使い方と意味は何か
        /// </summary>
        static void CheckAddressOfManagedAndUnmanaged()
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

        /// <summary>
        /// Pointerの操作
        /// </summary>
        static void ManipulatePointer()
        {
            int n;
            int* pn = &n;
            byte* p = (byte*) pn;
            *p = 0x78;  // nの最初の1バイト目に0x78を代入
            ++p;
            *p = 0x56;  // nの2バイト目に0x56を代入
            ++p;
            *p = 0x34;  // nの3バイト目に0x34を代入
            ++p;
            *p = 0x12;  // nの4バイト目に0x12を代入
            
            Console.WriteLine($"{n:X}");
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        readonly struct ValueData
        {
            public readonly ushort ShortNumber;

            public readonly uint IntNumber;

            public readonly byte ByteNumber;

            public ValueData(ushort shortNumber, uint intNumber, byte byteNumber)
            {
                ShortNumber = shortNumber;
                IntNumber = intNumber;
                ByteNumber = byteNumber;
            }
        }

        /// <summary>
        /// Structのメモリレイアウトを指定して、ポインタを操作して直接値を書き込む
        /// </summary>
        static void DirectStructAccess()
        {
            var data = new ValueData();
            var p = &data;
            byte* pp = (byte*) p;
            *pp = 0x21;
            ++pp;
            *pp = 0x43;
            // この時点でShortNumberの値は0x4321
            ++pp;
            *pp = 0x21;
            ++pp;
            *pp = 0x43;
            ++pp;
            *pp = 0x65;
            ++pp;
            *pp = 0x87;
            // この時点でIntNumberの値は0x87654321
            ++pp;
            *pp = 0x21;
            Console.WriteLine($"ShortNumber: {p->ShortNumber:X}");
            Console.WriteLine($"IntNumber: {p->IntNumber:X}");
            Console.WriteLine($"ByteNumber: {p->ByteNumber:X}");
        }

        /// <summary>
        /// Heapではなくstack上に配列を確保するstackalloc
        /// </summary>
        static void StackAllocSample()
        {
            const int N = 10;
            int* stackArray = stackalloc int[N];
            for (var i = 0; i < N; i++)
            {
                stackArray[i] = i;
            }

            for (var i = 0; i < N; i++)
            {
                Console.WriteLine($"stackArray[{i}]: {stackArray[i]}");
            }
        }
        
        static void Main(string[] args)
        {
            // CheckAddressOfManagedAndUnmanaged();
            // ManipulatePointer();
            // DirectStructAccess();
            // StackAllocSample();
            // SpanUsage.ArrayAsSpan();
            // SpanUsage.VariousMemorySpace();
            // var array = new byte[256];
            // SpanUsage.ClearArrayAll(array);

            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(SpanStringSubstring)
            });
            switcher.Run(args);
        }
    }

    class Data
    {
        public int Value;
    }

    public class SafeContextClass
    {
        public static void SafeStackAllocSampleBySpan()
        {
            Span<int> s = stackalloc int[5];
        }
    }
}
