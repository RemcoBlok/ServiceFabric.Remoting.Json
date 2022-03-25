using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonMessageBodyFactory : IServiceRemotingMessageBodyFactory
    {
        public IServiceRemotingRequestMessageBody CreateRequest(string interfaceName, string methodName, int numberOfParameters, object wrappedRequestObject)
        {
            return new ServiceRemotingJsonRequestMessageBody(numberOfParameters);
        }

        public IServiceRemotingResponseMessageBody CreateResponse(string interfaceName, string methodName, object wrappedResponseObject)
        {
            return new ServiceRemotingJsonResponseMessageBody();
        }
    }
}
