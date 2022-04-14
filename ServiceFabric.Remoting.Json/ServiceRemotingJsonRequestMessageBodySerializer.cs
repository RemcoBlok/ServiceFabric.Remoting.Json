using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonRequestMessageBodySerializer : IServiceRemotingRequestMessageBodySerializer
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

        public IServiceRemotingRequestMessageBody Deserialize(IIncomingMessageBody messageBody)
        {
            if (messageBody == null || messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
            {
                return null;
            }

            var serviceRemotingRequestMessageBody = JsonSerializer.Deserialize<ServiceRemotingJsonRequestMessageBody>(messageBody.GetReceivedBuffer(), Options);
            return serviceRemotingRequestMessageBody;
        }

        public IOutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            if (serviceRemotingRequestMessageBody == null)
            {
                return null;
            }

            var bytes = JsonSerializer.SerializeToUtf8Bytes<object>(serviceRemotingRequestMessageBody, Options);
            var segment = new ArraySegment<byte>(bytes);
            var segments = new[] { segment };
            return new OutgoingMessageBody(segments);
        }
    }
}
