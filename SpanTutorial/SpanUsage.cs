using System;
using System.Runtime.InteropServices;

namespace UnmanagedSample.SpanTutorial
{
    /// <summary>
    /// Spanの使い方について
    /// </summary>
    public class SpanUsage
    {
        /// <summary>
        /// 配列の一部分を切り取る
        /// </summary>
        public static void ArrayAsSpan()
        {
            var array = new int[8];
            
            // array[2] ~ array[5]までの範囲を切り取る
            var span = new Span<int>(array, 2, 3);

            // その範囲だけ1で上書き
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = 1;
            }

            foreach (var i in array)
            {
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// 色々なタイプのメモリ領域をさせる
        /// </summary>
        public static void VariousMemorySpace()
        {
            // 配列
            Span<int> array = new int[8].AsSpan().Slice(2, 3);
            
            // 文字列
            ReadOnlySpan<char> str = "abcdefg".AsSpan().Slice(2, 3);
            
            // スタック領域
            Span<int> stack = stackalloc int[8];

            unsafe
            {
                // .NET管理外メモリ
                var p = Marshal.AllocHGlobal(sizeof(int) * 8);
                Span<int> unmanaged = new Span<int>((int*) p, 8);
            }
        }

        /// <summary>
        /// 文字列のSubstringをコピーなしで実行できる
        /// </summary>
        public static void SubstringBySpan()
        {
            var s = "This is a pen!";
            var sub = s.AsSpan().Slice(3, 3);
            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine(sub[i]);
            }
        }

        /// <summary>
        /// 配列の全てをclearする
        /// </summary>
        /// <param name="span">Span</param>
        public static void ClearArrayAll(Span<byte> span)
        {
            unsafe
            {
                fixed (byte* pin = &span.GetPinnableReference())
                {
                    var p = pin;
                    var last = p + span.Length;
                    while (p + 7 < last)
                    {
                        *(ulong*) p = 0;
                        p += 8;
                    }

                    if (p + 3 < last)
                    {
                        *(uint*) p = 0;
                        p += 4;
                    }

                    while (p < last)
                    {
                        *p = 0;
                        ++p;
                    }
                }
            }
        }
    }
}