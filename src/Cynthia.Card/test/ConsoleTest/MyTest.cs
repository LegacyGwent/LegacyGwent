using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card;
using Cynthia.Card.AI;
using Cynthia.Card.Server;
using MongoDB.Driver;
using static System.Runtime.InteropServices.MemoryMarshal;

namespace ConsoleTest
{
    public static class MyTest
    {
    }

    public static class Encoder
    {
        private const string s_charMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-.";

        private const char s_noZip = '~';

        private static readonly byte[] s_head = { 31, 139, 8, 0, 0, 0, 0, 0, 0, 10 };

        private static readonly byte[] s_tail = { 0, 0, 255, 255 };

        public static string Encode(this string source) => Encode((ReadOnlySpan<char>)source.AsSpan());

        public static string Encode<T>(this T[] source) where T : unmanaged => Encode((ReadOnlySpan<T>)source.AsSpan());

        public static string Encode<T>(this Span<T> source) where T : unmanaged => Encode((ReadOnlySpan<T>)source);

        public static string Encode<T>(this ReadOnlySpan<T> source)
        where T : unmanaged
        {
            var sb = new StringBuilder();
            using var ms = new MemoryStream();
            using var gzip = new GZipStream(ms, CompressionLevel.Optimal, true);
            unsafe
            {
                fixed (T* ptr = source)
                {
                    var origin = new Span<byte>((byte*)ptr, source.Length * sizeof(T));
                    gzip.Write(origin);
                    gzip.Flush();
                    Debug.Assert(ms.GetBuffer().AsSpan().Slice(0, s_head.Length).SequenceEqual(s_head.AsSpan()));
                    Debug.Assert(ms.GetBuffer().AsSpan().Slice((int)ms.Length - s_tail.Length, s_tail.Length).SequenceEqual(s_tail.AsSpan()));
                    var useZipped = ms.Length < origin.Length;
                    var optimized = useZipped ? ms.GetBuffer().AsSpan().Slice(s_head.Length, (int)(ms.Length - s_head.Length - s_tail.Length)) : origin;

                    if (!useZipped)
                    {
                        sb.Append(s_noZip);
                    }

                    var buffer = 0u;
                    var span = new Span<byte>(&buffer, 3);

                    while (true)
                    {
                        if (optimized.Length == 0)
                        {
                            break;
                        }

                        var read = optimized.Length > 3 ? 3 : optimized.Length;
                        optimized.Slice(0, read).CopyTo(span);

                        for (var i = 0; i < read + 1; i++)
                        {
                            var value = (int)((buffer >> (6 * i)) & (0x3Fu));
                            sb.Append(s_charMap[value]);
                        }

                        buffer = 0u;
                        optimized = optimized.Slice(read);
                    }
                }
            }

            return sb.ToString();
        }

        public static string DecodeString(this string source) => new string(source.Decode<char>().ToArray());

        public static IEnumerable<T> Decode<T>(this string source)
        where T : unmanaged
        {
            static Stream ReadString(string source)
            {
                var ms = new MemoryStream();
                var useZipped = source[0] != s_noZip;

                if (useZipped)
                {
                    ms.Write(s_head, 0, s_head.Length);
                }

                for (var i = useZipped ? 0 : 1; i < source.Length; i += 4)
                {
                    var buffer = 0u;
                    var read = source.Length - i;
                    read = read > 4 ? 4 : read;

                    for (var j = 0; j < read; j++)
                    {
                        var value = s_charMap.IndexOf(source[i + j]);

                        if (value < 0)
                        {
                            throw new FormatException();
                        }

                        buffer |= ((uint)value) << (6 * j);
                    }

                    unsafe
                    {
                        var span = new Span<byte>(&buffer, read - 1);
                        ms.Write(span);
                    }
                }

                if (useZipped)
                {
                    ms.Write(s_tail, 0, s_tail.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    return new GZipStream(ms, CompressionMode.Decompress);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
            }

            static bool TryReadStream(Stream stream, out T value)
            {
                unsafe
                {
                    fixed (T* ptr = &value)
                    {
                        var span = new Span<byte>(ptr, sizeof(T));
                        var read = stream.Read(span);

                        if (read == 0)
                        {
                            return false;
                        }

                        if (read != sizeof(T))
                        {
                            throw new FormatException();
                        }

                        return true;
                    }
                }
            }

            using var stream = ReadString(source);

            while (TryReadStream(stream, out var result))
            {
                yield return result;
            }
        }

        public static Span<byte> Strip<T>(this Span<T> data, int bitLength)
        where T : unmanaged
        => Strip((ReadOnlySpan<T>)data, bitLength);

        public static Span<byte> Strip<T>(this ReadOnlySpan<T> data, int bitLength)
        where T : unmanaged
        {
            var result = new byte[(int)Math.Ceiling(data.Length * bitLength / (double)_bitCountOfByte)];
            var bytes = AsBytes(data);

            for (var i = 0; i < data.Length; i++)
            {
                unsafe
                {
                    BitwiseCopy(bytes, i * sizeof(T) * _bitCountOfByte, bitLength, result, i * bitLength);
                }
            }

            return result;
        }

        public static T[] Expand<T>(this Span<byte> data, int bitLength)
        where T : unmanaged
        => Expand<T>((ReadOnlySpan<byte>)data, bitLength);

        public static T[] Expand<T>(this ReadOnlySpan<byte> data, int bitLength)
        where T : unmanaged
        {
            var result = new T[data.Length * _bitCountOfByte / bitLength];
            var bytes = AsBytes(result.AsSpan());

            for (var i = 0; i < result.Length; i++)
            {
                unsafe
                {
                    BitwiseCopy(data, i * bitLength, bitLength, bytes, i * sizeof(T) * _bitCountOfByte);
                }
            }

            return result;
        }


        private const int _bitCountOfByte = 8;

        public static void BitwiseCopy(ReadOnlySpan<byte> src, int srcBitOffset, int bitLength, Span<byte> des, int desBitOffset)
        {
            unsafe
            {
                while (bitLength > 0)
                {
                    if (srcBitOffset >= _bitCountOfByte)
                    {
                        src = src.Slice(srcBitOffset / _bitCountOfByte);
                        srcBitOffset %= _bitCountOfByte;
                    }

                    var copyLength = _bitCountOfByte - srcBitOffset % _bitCountOfByte;

                    if (copyLength > bitLength)
                    {
                        copyLength = bitLength;
                    }

                    var b = (byte)((byte)(src[0] << (_bitCountOfByte - copyLength - srcBitOffset)) >> (_bitCountOfByte - copyLength));

                    if (desBitOffset >= _bitCountOfByte)
                    {
                        des = des.Slice(desBitOffset / _bitCountOfByte);
                        desBitOffset %= _bitCountOfByte;
                    }

                    var desTil = desBitOffset + copyLength;

                    var desFirstLength = desTil > _bitCountOfByte ? _bitCountOfByte - desBitOffset : copyLength;
                    des[0] = (byte)(((byte)(des[0] << desFirstLength)) >> desFirstLength | b << desBitOffset);

                    if (desFirstLength < copyLength)
                    {
                        var desRestLength = copyLength - desFirstLength;
                        des[1] = (byte)(des[1] >> desRestLength << desRestLength | b >> desFirstLength);
                    }

                    srcBitOffset += copyLength;
                    desBitOffset += copyLength;
                    bitLength -= copyLength;
                }
            }
        }
    }
}