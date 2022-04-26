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
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new ActorConverter(),
                new MessageConverter()
            }
        };

        private readonly IBufferPoolManager _bufferPoolManager;

        public ServiceRemotingJsonRequestMessageBodySerializer(IBufferPoolManager bufferPoolManager)
        {
            _bufferPoolManager = bufferPoolManager;
        }

        public IServiceRemotingRequestMessageBody Deserialize(IIncomingMessageBody messageBody)
        {
            if (messageBody == null)
            {
                return null;
            }

            var receivedBufferStream = messageBody.GetReceivedBuffer();
            if (receivedBufferStream == null || receivedBufferStream.Length == 0)
            {
                return null;
            }

            using (receivedBufferStream)
            {
                var serviceRemotingRequestMessageBody = JsonSerializer.Deserialize<ServiceRemotingJsonRequestMessageBody>(receivedBufferStream, Options);
                return serviceRemotingRequestMessageBody;
            }
        }

        public IOutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            if (serviceRemotingRequestMessageBody == null)
            {
                return null;
            }

            using var stream = new SegmentedPoolMemoryStream(_bufferPoolManager);
            using var writer = new Utf8JsonWriter(stream);

            JsonSerializer.Serialize<object>(writer, serviceRemotingRequestMessageBody, Options);

            var outgoingPooledBodyBuffers = stream.GetBuffers();
            return new OutgoingMessageBody(outgoingPooledBodyBuffers);
        }
    }
}
