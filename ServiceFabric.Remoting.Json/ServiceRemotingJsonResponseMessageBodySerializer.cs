using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonResponseMessageBodySerializer : IServiceRemotingResponseMessageBodySerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateOnlyConverter(),
                new DateOnlyNullableConverter(),
                new ActorConverter(),
                new MessageConverter()
            }
        };

        public IServiceRemotingResponseMessageBody Deserialize(IIncomingMessageBody messageBody)
        {
            if (messageBody == null || messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
            {
                return null;
            }

            var serviceRemotingResponseMessageBody = JsonSerializer.Deserialize<ServiceRemotingJsonResponseMessageBody>(messageBody.GetReceivedBuffer(), Options);
            return serviceRemotingResponseMessageBody;
        }

        public IOutgoingMessageBody Serialize(IServiceRemotingResponseMessageBody serviceRemotingResponseMessageBody)
        {
            if (serviceRemotingResponseMessageBody == null)
            {
                return null;
            }

            var bytes = JsonSerializer.SerializeToUtf8Bytes<object>(serviceRemotingResponseMessageBody, Options);
            var segment = new ArraySegment<byte>(bytes);
            var segments = new[] { segment };
            return new OutgoingMessageBody(segments);
        }
    }
}
