using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public sealed class SignalRJsonProtocolCompatibilityTests
    {
        [Fact]
        public void StartupConfiguredProtocol_WritesAsciiOnlyAndRoundTripsEveryStringToken()
        {
            using var provider = CreateServerServices();
            var protocol = GetJsonProtocol(provider);
            var payload = new TestEnvelope
            {
                Message = "中文牌组 🚀 / Zażółć gęślą jaźń" + new string('界', 16_384),
                Nested = new TestNested
                {
                    Label = "嵌套值 🃏",
                    Values = new[] { "第一项", "emoji: 😀", "plain ASCII" }
                },
                Metadata = new Dictionary<string, string>
                {
                    ["中文键 🔑"] = "简体中文",
                    ["状态"] = "兼容通过 ✅"
                }
            };

            var bytes = WriteInvocation(protocol, payload);

            Assert.Equal(0x1e, bytes[bytes.Length - 1]);
            Assert.All(bytes, value => Assert.InRange(value, (byte)0, (byte)0x7f));
            Assert.Contains("\\u", Encoding.UTF8.GetString(bytes));

            var invocation = ParseInvocation(protocol, bytes);
            var roundTripped = Assert.IsType<TestEnvelope>(Assert.Single(invocation.Arguments));
            Assert.Equal(payload.Message, roundTripped.Message);
            Assert.Equal(payload.Nested.Label, roundTripped.Nested.Label);
            Assert.Equal(payload.Nested.Values, roundTripped.Nested.Values);
            Assert.Equal(payload.Metadata, roundTripped.Metadata);
        }

        [Fact]
        public void StartupConfiguredProtocol_ReadsLiteralUtf8PropertyNamesKeysAndValues()
        {
            using var provider = CreateServerServices();
            var protocol = GetJsonProtocol(provider);
            const string frame =
                "{\"type\":1,\"target\":\"ReceivePayload\",\"arguments\":[{" +
                "\"中文字段\":\"原始中文 🚀\",\"nested\":{" +
                "\"label\":\"嵌套值 🃏\",\"values\":[\"第一项\"]}," +
                "\"metadata\":{\"中文键 🔑\":\"兼容通过 ✅\"}}]}\u001e";

            var invocation = ParseInvocation(protocol, Encoding.UTF8.GetBytes(frame));
            var payload = Assert.IsType<TestEnvelope>(Assert.Single(invocation.Arguments));

            Assert.Equal("原始中文 🚀", payload.Message);
            Assert.Equal("嵌套值 🃏", payload.Nested.Label);
            Assert.Equal(new[] { "第一项" }, payload.Nested.Values);
            Assert.Equal("兼容通过 ✅", payload.Metadata["中文键 🔑"]);
        }

        [Fact]
        public void StartupConfiguredProtocol_WritesAsciiOnlyAndRoundTripsEnvelopeStrings()
        {
            using var provider = CreateServerServices();
            var protocol = GetJsonProtocol(provider);
            var output = new ArrayBufferWriter<byte>();
            protocol.WriteMessage(
                new InvocationMessage("调用-一", "接收载荷 🚀", Array.Empty<object>()),
                output);
            var bytes = output.WrittenSpan.ToArray();

            Assert.All(bytes, value => Assert.InRange(value, (byte)0, (byte)0x7f));

            var invocation = ParseInvocation(protocol, bytes);
            Assert.Equal("调用-一", invocation.InvocationId);
            Assert.Equal("接收载荷 🚀", invocation.Target);
            Assert.Empty(invocation.Arguments);
        }

        [Fact]
        public void StartupConfiguredProtocol_WritesAsciiOnlyAndRoundTripsCompletionErrors()
        {
            using var provider = CreateServerServices();
            var protocol = GetJsonProtocol(provider);
            var bytes = protocol.GetMessageBytes(
                CompletionMessage.WithError("调用-二", "服务器错误：中文 🚀")).ToArray();

            Assert.All(bytes, value => Assert.InRange(value, (byte)0, (byte)0x7f));

            var completion = Assert.IsType<CompletionMessage>(ParseMessage(protocol, bytes));
            Assert.Equal("调用-二", completion.InvocationId);
            Assert.Equal("服务器错误：中文 🚀", completion.Error);
        }

        [Fact]
        public void GetMessageBytesAsciiFastPath_PreservesTheFrameworkFrameExactly()
        {
            using var provider = CreateServerServices();
            var protocol = GetJsonProtocol(provider);
            var frameworkProtocol = new JsonHubProtocol(
                provider.GetRequiredService<IOptions<JsonHubProtocolOptions>>());
            var message = new InvocationMessage(
                "ascii-invocation",
                "ReceiveAsciiPayload",
                new object[] { "plain ASCII", new Dictionary<string, string> { ["key"] = "value" } });

            var expected = frameworkProtocol.GetMessageBytes(message);
            var actual = protocol.GetMessageBytes(message);

            Assert.Equal(expected.ToArray(), actual.ToArray());
            Assert.All(actual.ToArray(), value => Assert.InRange(value, (byte)0, (byte)0x7f));
        }

        [Fact]
        public void StartupConfiguration_PreservesTheGwentHubClientTimeout()
        {
            using var provider = CreateServerServices();

            var options = provider.GetRequiredService<IOptions<HubOptions<GwentHub>>>().Value;
            var protocols = provider.GetServices<IHubProtocol>().ToArray();

            Assert.Equal(TimeSpan.FromSeconds(90), options.ClientTimeoutInterval);
            Assert.Single(protocols, protocol => protocol.Name == "json");
            Assert.Contains(protocols, protocol => protocol.Name == "blazorpack");
        }

        private static ServiceProvider CreateServerServices()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            new Startup(null).ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private static IHubProtocol GetJsonProtocol(IServiceProvider provider)
        {
            return provider.GetServices<IHubProtocol>().Single(protocol => protocol.Name == "json");
        }

        private static byte[] WriteInvocation(IHubProtocol protocol, TestEnvelope payload)
        {
            var output = new ArrayBufferWriter<byte>();
            protocol.WriteMessage(
                new InvocationMessage("compatibility-test", "ReceivePayload", new object[] { payload }),
                output);
            return output.WrittenSpan.ToArray();
        }

        private static InvocationMessage ParseInvocation(IHubProtocol protocol, byte[] bytes)
        {
            return Assert.IsType<InvocationMessage>(ParseMessage(protocol, bytes));
        }

        private static HubMessage ParseMessage(IHubProtocol protocol, byte[] bytes)
        {
            var input = new ReadOnlySequence<byte>(bytes);
            Assert.True(protocol.TryParseMessage(ref input, TestInvocationBinder.Instance, out var message));
            Assert.True(input.IsEmpty);
            return message;
        }

        private sealed class TestInvocationBinder : IInvocationBinder
        {
            public static TestInvocationBinder Instance { get; } = new TestInvocationBinder();

            public IReadOnlyList<Type> GetParameterTypes(string methodName)
            {
                if (methodName == "ReceivePayload")
                {
                    return new[] { typeof(TestEnvelope) };
                }

                if (methodName == "接收载荷 🚀")
                {
                    return Array.Empty<Type>();
                }

                throw new InvalidOperationException("Unexpected hub method: " + methodName);
            }

            public Type GetReturnType(string invocationId)
            {
                throw new NotSupportedException();
            }

            public Type GetStreamItemType(string streamId)
            {
                throw new NotSupportedException();
            }
        }

        public sealed class TestEnvelope
        {
            [JsonPropertyName("中文字段")]
            public string Message { get; set; }

            public TestNested Nested { get; set; }

            public Dictionary<string, string> Metadata { get; set; }
        }

        public sealed class TestNested
        {
            public string Label { get; set; }

            public string[] Values { get; set; }
        }
    }
}
