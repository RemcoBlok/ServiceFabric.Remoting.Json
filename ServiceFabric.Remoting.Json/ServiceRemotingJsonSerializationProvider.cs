using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new ServiceRemotingJsonMessageBodyFactory();
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(Type serviceInterfaceType, IEnumerable<Type> requestWrappedTypes, IEnumerable<Type> requestBodyTypes = null)
        {
            return new ServiceRemotingJsonRequestMessageBodySerializer();
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(Type serviceInterfaceType, IEnumerable<Type> responseWrappedTypes, IEnumerable<Type> responseBodyTypes = null)
        {
            return new ServiceRemotingJsonResponseMessageBodySerializer();
        }
    }
}
