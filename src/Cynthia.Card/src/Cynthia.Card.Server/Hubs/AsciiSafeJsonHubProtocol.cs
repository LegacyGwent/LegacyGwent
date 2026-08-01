using System;
using System.Buffers;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Options;

namespace Cynthia.Card.Server
{
    /// <summary>
    /// Delegates JSON parsing and message construction to the framework protocol, then
    /// escapes any remaining non-ASCII UTF-8 in each complete post-handshake hub frame.
    /// Payload converters cannot reach protocol-owned strings such as target, invocation
    /// ID, and error text, so the final frame must be normalized at this boundary.
    /// </summary>
    internal sealed class AsciiSafeJsonHubProtocol : IHubProtocol
    {
        private static readonly byte[] HexDigits = Encoding.ASCII.GetBytes("0123456789ABCDEF");
        private readonly JsonHubProtocol _inner;

        public AsciiSafeJsonHubProtocol(IOptions<JsonHubProtocolOptions> options)
        {
            _inner = new JsonHubProtocol(options);
        }

        public string Name => _inner.Name;

        public int Version => _inner.Version;

        public TransferFormat TransferFormat => _inner.TransferFormat;

        public bool IsVersionSupported(int version) => _inner.IsVersionSupported(version);

        public bool TryParseMessage(
            ref ReadOnlySequence<byte> input,
            IInvocationBinder binder,
            out HubMessage message)
        {
            return _inner.TryParseMessage(ref input, binder, out message);
        }

        public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
        {
            var frame = _inner.GetMessageBytes(message);
            if (IsAscii(frame.Span))
            {
                WriteBytes(output, frame.Span);
                return;
            }

            var escapedLength = GetAsciiSafeLength(frame.Span);
            var destination = output.GetSpan(escapedLength).Slice(0, escapedLength);
            WriteAsciiSafe(frame.Span, destination);
            output.Advance(escapedLength);
        }

        public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
        {
            var frame = _inner.GetMessageBytes(message);
            if (IsAscii(frame.Span))
            {
                return frame;
            }

            var output = new byte[GetAsciiSafeLength(frame.Span)];
            WriteAsciiSafe(frame.Span, output);
            return output;
        }

        private static bool IsAscii(ReadOnlySpan<byte> source)
        {
            foreach (var value in source)
            {
                if (value > 0x7f)
                {
                    return false;
                }
            }

            return true;
        }

        private static int GetAsciiSafeLength(ReadOnlySpan<byte> source)
        {
            var length = source.Length;
            var offset = 0;

            while (offset < source.Length)
            {
                if (source[offset] <= 0x7f)
                {
                    offset++;
                    continue;
                }

                var rune = DecodeRune(source.Slice(offset), out var consumed);
                length = checked(length + GetEscapedRuneLength(rune) - consumed);
                offset += consumed;
            }

            return length;
        }

        private static void WriteAsciiSafe(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            var sourceOffset = 0;
            var destinationOffset = 0;

            while (sourceOffset < source.Length)
            {
                if (source[sourceOffset] <= 0x7f)
                {
                    destination[destinationOffset++] = source[sourceOffset++];
                    continue;
                }

                var rune = DecodeRune(source.Slice(sourceOffset), out var consumed);
                destinationOffset += WriteEscapedRune(rune, destination.Slice(destinationOffset));
                sourceOffset += consumed;
            }

            if (destinationOffset != destination.Length)
            {
                throw new InvalidDataException("ASCII-safe SignalR frame length mismatch.");
            }
        }

        private static Rune DecodeRune(ReadOnlySpan<byte> source, out int consumed)
        {
            var status = Rune.DecodeFromUtf8(source, out var rune, out consumed);
            if (status != OperationStatus.Done)
            {
                throw new InvalidDataException("JsonHubProtocol produced invalid UTF-8.");
            }

            return rune;
        }

        private static int GetEscapedRuneLength(Rune rune)
        {
            return rune.IsBmp ? 6 : 12;
        }

        private static int WriteEscapedRune(Rune rune, Span<byte> destination)
        {
            if (rune.IsBmp)
            {
                WriteEscapedCodeUnit((ushort)rune.Value, destination);
                return 6;
            }

            var scalar = rune.Value - 0x10000;
            var highSurrogate = (ushort)(0xd800 + (scalar >> 10));
            var lowSurrogate = (ushort)(0xdc00 + (scalar & 0x3ff));
            WriteEscapedCodeUnit(highSurrogate, destination);
            WriteEscapedCodeUnit(lowSurrogate, destination.Slice(6));
            return 12;
        }

        private static void WriteEscapedCodeUnit(ushort value, Span<byte> destination)
        {
            destination[0] = (byte)'\\';
            destination[1] = (byte)'u';
            destination[2] = HexDigits[(value >> 12) & 0xf];
            destination[3] = HexDigits[(value >> 8) & 0xf];
            destination[4] = HexDigits[(value >> 4) & 0xf];
            destination[5] = HexDigits[value & 0xf];
        }

        private static void WriteBytes(IBufferWriter<byte> output, ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty)
            {
                return;
            }

            var destination = output.GetSpan(bytes.Length);
            bytes.CopyTo(destination);
            output.Advance(bytes.Length);
        }
    }
}
