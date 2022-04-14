using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ServiceFabric.Remoting.Json.Tests
{
    public class ServiceRemotingJsonSerializationProviderTests
    {
        [Fact]
        public void WhenSendingAndReceivingRequest_ShouldSerializeAndDeserialze()
        {
            // Arrange
            var sut = new ServiceRemotingJsonSerializationProvider();
            var messageBodyFactory = sut.CreateMessageBodyFactory();
            var serializer = sut.CreateRequestMessageSerializer(typeof(ITestActor), null);

            var command = new TestCommand
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-1)
            };

            var request = messageBodyFactory.CreateRequest(nameof(ITestActor), nameof(ITestActor.TestOperation), 1, null);
            request.SetParameter(0, "command", command);

            // Act
            using var outgoingMessageBody = serializer.Serialize(request);

            var segments = outgoingMessageBody.GetSendBuffers();
            var bytes = segments.SelectMany(s => s.Array).ToArray();

            using var receivedBufferStream = new MemoryStream(bytes);
            using var incomingMessageBody = new IncomingMessageBody(receivedBufferStream);

            var receivedRequest = serializer.Deserialize(incomingMessageBody);
            var receivedCommand = receivedRequest.GetParameter(0, "command", typeof(TestCommand));

            // Assert
            receivedCommand.Should().BeOfType<TestCommand>().Which.Should().BeEquivalentTo(command);
        }

        [Fact]
        public void WhenSendingAndReceivingResponse_ShouldSerializeAndDeserialze()
        {
            // Arrange
            var sut = new ServiceRemotingJsonSerializationProvider();
            var messageBodyFactory = sut.CreateMessageBodyFactory();
            var serializer = sut.CreateResponseMessageSerializer(typeof(ITestActor), null);

            var responseValue = true;

            var response = messageBodyFactory.CreateResponse(nameof(ITestActor), nameof(ITestActor.TestOperation), null);
            response.Set(responseValue);

            // Act
            using var outgoingMessageBody = serializer.Serialize(response);

            var segments = outgoingMessageBody.GetSendBuffers();
            var bytes = segments.SelectMany(s => s.Array).ToArray();

            using var receivedBufferStream = new MemoryStream(bytes);
            using var incomingMessageBody = new IncomingMessageBody(receivedBufferStream);

            var receivedResponse = serializer.Deserialize(incomingMessageBody);
            var receivedResponseValue = receivedResponse.Get(typeof(bool));

            // Assert
            receivedResponseValue.Should().BeOfType<bool>().Which.Should().Be(responseValue);
        }
    }

    public interface ITestActor : IActor
    {
        Task<bool> TestOperation(TestCommand command, CancellationToken cancellationToken);
    }

    public record TestCommand
    {
        public DateOnly DateOfBirth { get; init; }
        public DateOnly? DeliveryDate { get; init; }
    }
}
