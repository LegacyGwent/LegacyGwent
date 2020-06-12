using System;

namespace CompressTest
{
    public readonly ref struct ReadOnlyBitSpan
    {
        public ReadOnlySpan<byte> Span { get; }

        public int Length { get; }

        public int Offset { get; }

        private const int s_bitCountOfByte = 8;

        public ReadOnlyBitSpan(ReadOnlySpan<byte> span, int offset = 0, int? length = default)
        {
            Span = span;
            Offset = offset;
            Length = length ?? span.Length * s_bitCountOfByte - offset;

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var endOffset = span.Length * s_bitCountOfByte - offset - Length;

            if (endOffset < 0)
            {
                throw new InsufficientMemoryException();
            }

            var skipStart = Offset / s_bitCountOfByte;
            var skipEnd = endOffset / s_bitCountOfByte;

            if (skipStart > 0 || skipEnd > 0)
            {
                Span = Span.Slice(skipStart, Span.Length - skipStart - skipEnd);
                Offset -= skipStart * s_bitCountOfByte;
            }
        }

        public ReadOnlyBitSpan Slice(int start) => new ReadOnlyBitSpan(Span, Offset + start, Length - start);

        public ReadOnlyBitSpan Slice(int start, int length) => new ReadOnlyBitSpan(Span, Offset + start, length);

        public static explicit operator BitSpan(ReadOnlyBitSpan span)
        {
            unsafe
            {
                fixed (byte* ptr = span.Span)
                {
                    return new BitSpan(new Span<byte>(ptr, span.Span.Length), span.Offset, span.Length);
                }
            }
        }

        public static implicit operator ReadOnlyBitSpan(ReadOnlySpan<byte> span) => new ReadOnlyBitSpan(span);

        public static implicit operator ReadOnlyBitSpan(Span<byte> span) => new ReadOnlyBitSpan(span);

        public static implicit operator ReadOnlyBitSpan(byte[] span) => new ReadOnlyBitSpan(span);

        public static implicit operator ReadOnlyBitSpan(BitSpan span) => new ReadOnlyBitSpan(span.Span, span.Offset, span.Length);
    }
}