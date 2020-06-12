using System;

namespace CompressTest
{
    public readonly ref struct BitSpan
    {
        public Span<byte> Span { get; }

        public int Length { get; }

        public int Offset { get; }

        private const int s_bitCountOfByte = 8;

        public BitSpan(Span<byte> span, int offset = 0, int? length = default)
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

        public BitSpan Slice(int start) => new BitSpan(Span, Offset + start, Length - start);

        public BitSpan Slice(int start, int length) => new BitSpan(Span, Offset + start, length);

        public BitSpan Write<T>(in T src, int? bitLength = default)
        where T : unmanaged
        {
            unsafe
            {
                fixed (T* ptr = &src)
                {
                    var original = sizeof(T) * s_bitCountOfByte;
                    var expected = bitLength ?? sizeof(T) * s_bitCountOfByte;

                    return expected > original ?
                        Write(new ReadOnlySpan<byte>(ptr, sizeof(T))).Slice(expected - original) :
                        Write(new ReadOnlyBitSpan(new ReadOnlySpan<byte>(ptr, sizeof(T)), length: expected));
                }
            }
        }

        public BitSpan Write(ReadOnlyBitSpan src)
        {
            unsafe
            {
                var des = this;

                while (src.Length > 0)
                {
                    var copyLength = s_bitCountOfByte - src.Offset % s_bitCountOfByte;

                    if (copyLength > src.Length)
                    {
                        copyLength = src.Length;
                    }

                    var b = (byte)((byte)(src.Span[0] << (s_bitCountOfByte - copyLength - src.Offset)) >> (s_bitCountOfByte - copyLength));

                    var desTil = des.Offset + copyLength;

                    var desFirstLength = desTil > s_bitCountOfByte ? s_bitCountOfByte - des.Offset : copyLength;
                    des.Span[0] = (byte)(((byte)(des.Span[0] << desFirstLength)) >> desFirstLength | b << des.Offset);

                    if (desFirstLength < copyLength)
                    {
                        var desRestLength = copyLength - desFirstLength;
                        des.Span[1] = (byte)(des.Span[1] >> desRestLength << desRestLength | b >> desFirstLength);
                    }

                    src = src.Slice(copyLength);
                    des = des.Slice(copyLength);
                }

                return des;
            }
        }

        public static implicit operator BitSpan(Span<byte> span) => new BitSpan(span);

        public static implicit operator BitSpan(byte[] span) => new BitSpan(span);
    }
}