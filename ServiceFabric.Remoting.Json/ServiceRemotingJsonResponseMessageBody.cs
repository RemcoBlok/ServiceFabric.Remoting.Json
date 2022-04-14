using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonResponseMessageBody : IServiceRemotingResponseMessageBody
    {
        public object Response { get; set; }

        public object Get(Type paramType)
        {
            return Response;
        }

        public void Set(object response)
        {
            Response = response;
        }
    }
}
