using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using System;
using System.Collections.Generic;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        private readonly IBufferPoolManager _bodyBufferPoolManager = new BufferPoolManager(Constants.DefaultMessageBufferSize, Constants.DefaultMaxBufferCount);

        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new ServiceRemotingJsonMessageBodyFactory();
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(Type serviceInterfaceType, IEnumerable<Type> requestWrappedTypes, IEnumerable<Type> requestBodyTypes = null)
        {
            return new ServiceRemotingJsonRequestMessageBodySerializer(_bodyBufferPoolManager);
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(Type serviceInterfaceType, IEnumerable<Type> responseWrappedTypes, IEnumerable<Type> responseBodyTypes = null)
        {
            return new ServiceRemotingJsonResponseMessageBodySerializer(_bodyBufferPoolManager);
        }
    }
}
