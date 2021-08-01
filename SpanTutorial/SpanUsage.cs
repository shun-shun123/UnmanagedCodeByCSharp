using System;

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
            

            
        }
    }
}