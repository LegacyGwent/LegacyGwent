using System;
using System.Collections.Generic;

namespace CompressTest
{
    public static class ByteSerializer
    {
        private const int s_bitCountOfByte = 8;

        public static int GetBitLength(int value)
        {
            if (value == 0)
            {
                return 0;
            }

            var i = 0;
            var sum = 1;

            while (sum - 1 < value)
            {
                sum *= 2;
                i++;
            }

            return i;
        }

        public static int GetMaxValueOfBitLength(int bitLength) => (int)Math.Pow(2, bitLength) - 1;

        private const int s_prefixInitialStepLength = 3;

        private static (int, int, int) GetFlexibleLengths(int length)
        {
            var fixed1Length = 0;
            var restLength = length;
            var prefixLength = s_prefixInitialStepLength;

            while (true)
            {
                var max = GetMaxValueOfBitLength(prefixLength);

                if (restLength < max)
                {
                    break;
                }

                restLength -= max;
                fixed1Length += prefixLength;
                prefixLength++;
            }

            return (fixed1Length, prefixLength, restLength);
        }

        public static int GetFlexibleLength(int value)
        {
            var length = GetBitLength(value);
            var (fixed1Length, prefixLength, _) = GetFlexibleLengths(length);
            return fixed1Length + prefixLength + (length > 1 ? length - 1 : 0);
        }

        public static int GetFlexibleLength(ICollection<int> value)
        => GetFlexibleLength(value, GetFlexibleLength);

        public static int GetFlexibleLength(ICollection<ICollection<int>> value)
        => GetFlexibleLength<int>(value, GetFlexibleLength);

        public static int GetFlexibleLength<T>(ICollection<ICollection<T>> value, Func<T, int> elemLength)
        => GetFlexibleLength(value, v => GetFlexibleLength(v, elemLength));

        public static int GetFlexibleLength<T>(ICollection<T> value, Func<T, int> elemLength)
        {
            var result = GetFlexibleLength(value.Count);

            foreach (var item in value)
            {
                result += elemLength(item);
            }

            return result;
        }

        public delegate BitSpan WriteAction<T>(BitSpan span, in T item);

        public delegate BitSpan WriteAndGetLengthAction<T>(BitSpan span, in T item, out int length);

        public static BitSpan WriteFlexible(this BitSpan span, in int value) => span.WriteFlexible(value, out _);

        public static BitSpan WriteFlexible(this BitSpan span, in int value, out int bitLength)
        {
            var length = GetBitLength(value);
            var (fixed1Length, prefixLength, restLength) = GetFlexibleLengths(length);
            bitLength = fixed1Length + prefixLength + (length > 1 ? length - 1 : 0);

            return span
                .Write(GetMaxValueOfBitLength(fixed1Length), fixed1Length)
                .Write(restLength, prefixLength)
                .Write(value, length - 1 > 0 ? length - 1 : 0);
        }

        public static BitSpan WriteFlexible(this BitSpan span, ICollection<int> items)
        => span.WriteFlexible(items, WriteFlexible);

        public static BitSpan WriteFlexible(this BitSpan span, ICollection<int> items, out int length)
        => span.WriteFlexible(items, WriteFlexible, out length);

        public static BitSpan WriteFlexible(this BitSpan span, ICollection<ICollection<int>> items)
        => span.WriteFlexible(items, WriteFlexible);

        public static BitSpan WriteFlexible(this BitSpan span, ICollection<ICollection<int>> items, out int length)
        => span.WriteFlexible(items, WriteFlexible, out length);

        public static BitSpan WriteFlexible<T>(this BitSpan span, ICollection<ICollection<T>> items, WriteAction<T> elemWriter)
        => span.WriteFlexible(items, (BitSpan span, in ICollection<T> item) => span.WriteFlexible(item, elemWriter));

        public static BitSpan WriteFlexible<T>(this BitSpan span, ICollection<ICollection<T>> items, WriteAndGetLengthAction<T> elemWriter, out int length)
        => span.WriteFlexible(items, (BitSpan span, in ICollection<T> item, out int length) => span.WriteFlexible(item, elemWriter, out length), out length);

        public static BitSpan WriteFlexible<T>(this BitSpan span, ICollection<T> items, WriteAction<T> elemWriter)
        {
            span = span.WriteFlexible(items.Count);

            foreach (var item in items)
            {
                span = elemWriter(span, item);
            }

            return span;
        }

        public static BitSpan WriteFlexible<T>(this BitSpan span, ICollection<T> items, WriteAndGetLengthAction<T> elemWriter, out int length)
        {
            span = span.WriteFlexible(items.Count, out length);

            foreach (var item in items)
            {
                span = elemWriter(span, item, out var elemLength);
                length += elemLength;
            }

            return span;
        }

        public delegate ReadOnlyBitSpan ReadAction<T>(ReadOnlyBitSpan span, out T item);

        public delegate ReadOnlyBitSpan ReadAndGetLengthAction<T>(ReadOnlyBitSpan span, out T item, out int length);

        public static BitSpan ReadFlexible(this BitSpan span, out int value)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, out _);

        public static BitSpan ReadFlexible(this BitSpan span, out int value, out int length)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, out length);

        public static BitSpan ReadFlexible(this BitSpan span, out int[] value)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value);

        public static BitSpan ReadFlexible(this BitSpan span, out int[] value, out int length)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, out length);

        public static BitSpan ReadFlexible(this BitSpan span, out int[][] value)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value);

        public static BitSpan ReadFlexible(this BitSpan span, out int[][] value, out int length)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, out length);

        public static BitSpan ReadFlexible<T>(this BitSpan span, out T[][] value, ReadAction<T> elemReader)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, elemReader);

        public static BitSpan ReadFlexible<T>(this BitSpan span, out T[][] value, ReadAndGetLengthAction<T> elemReader, out int length)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, elemReader, out length);

        public static BitSpan ReadFlexible<T>(this BitSpan span, out T[] value, ReadAction<T> elemReader)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, elemReader);

        public static BitSpan ReadFlexible<T>(this BitSpan span, out T[] value, ReadAndGetLengthAction<T> elemReader, out int length)
        => (BitSpan)ReadFlexible((ReadOnlyBitSpan)span, out value, elemReader, out length);

        public static ReadOnlyBitSpan ReadFlexible(this ReadOnlyBitSpan span, out int value)
        => span.ReadFlexible(out value, out _);

        public static ReadOnlyBitSpan ReadFlexible(this ReadOnlyBitSpan span, out int value, out int length)
        {
            length = 0;
            var prefixLength = s_prefixInitialStepLength;
            var totalPrefixLength = 0;
            var read = 0;
            var readSpan = default(BitSpan);
            unsafe
            {
                readSpan = new Span<byte>(&read, sizeof(uint));
            }

            while (true)
            {
                readSpan.Write(span.Slice(0, prefixLength));
                span = span.Slice(prefixLength);
                length += read;
                totalPrefixLength += prefixLength;
                var max = GetMaxValueOfBitLength(prefixLength);

                if (read < max)
                {
                    break;
                }

                read = 0;
                prefixLength++;
            }

            read = 0;
            readSpan.Write(span.Slice(0, length - 1)).Write(1, 1);
            span = span.Slice(length - 1);
            value = read;

            length += totalPrefixLength;
            return span;
        }

        public static ReadOnlyBitSpan ReadFlexible(this ReadOnlyBitSpan span, out int[] value)
        => span.ReadFlexible(out value, ReadFlexible);

        public static ReadOnlyBitSpan ReadFlexible(this ReadOnlyBitSpan span, out int[] value, out int length)
        => span.ReadFlexible(out value, ReadFlexible, out length);

        public static ReadOnlyBitSpan ReadFlexible(this ReadOnlyBitSpan span, out int[][] value)
        => span.ReadFlexible<int>(out value, ReadFlexible);

        public static ReadOnlyBitSpan ReadFlexible(this ReadOnlyBitSpan span, out int[][] value, out int length)
        => span.ReadFlexible<int>(out value, ReadFlexible, out length);

        public static ReadOnlyBitSpan ReadFlexible<T>(this ReadOnlyBitSpan span, out T[][] value, ReadAction<T> elemReader)
        => span.ReadFlexible(out value, (ReadOnlyBitSpan span, out T[] item) => span.ReadFlexible<T>(out item, elemReader));

        public static ReadOnlyBitSpan ReadFlexible<T>(this ReadOnlyBitSpan span, out T[][] value, ReadAndGetLengthAction<T> elemReader, out int length)
        => span.ReadFlexible(out value, (ReadOnlyBitSpan span, out T[] item, out int length) => span.ReadFlexible<T>(out item, elemReader, out length), out length);

        public static ReadOnlyBitSpan ReadFlexible<T>(this ReadOnlyBitSpan span, out T[] value, ReadAction<T> elemReader)
        {
            span = span.ReadFlexible(out int read);
            value = new T[read];

            for (var i = 0; i < value.Length; i++)
            {
                span = elemReader(span, out value[i]);
            }

            return span;
        }

        public static ReadOnlyBitSpan ReadFlexible<T>(this ReadOnlyBitSpan span, out T[] value, ReadAndGetLengthAction<T> elemReader, out int length)
        {
            span = span.ReadFlexible(out int read, out length);
            value = new T[read];

            for (var i = 0; i < value.Length; i++)
            {
                span = elemReader(span, out value[i], out var elemLength);
                length += elemLength;
            }

            return span;
        }
    }
}