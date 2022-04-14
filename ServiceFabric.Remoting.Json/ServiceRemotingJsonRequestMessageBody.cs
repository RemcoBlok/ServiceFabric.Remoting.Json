using Microsoft.ServiceFabric.Services.Remoting.V2;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class ServiceRemotingJsonRequestMessageBody : IServiceRemotingRequestMessageBody
    {
        public ServiceRemotingJsonRequestMessageBody()
        {
            Parameters = new Dictionary<string, object>();
        }

        public ServiceRemotingJsonRequestMessageBody(int numberOfParameters)
        {
            Parameters = new Dictionary<string, object>(numberOfParameters);
        }

        [JsonInclude]
        public Dictionary<string, object> Parameters { get; private set; }

        public object GetParameter(int position, string parameName, Type paramType)
        {
            return Parameters[parameName];
        }

        public void SetParameter(int position, string parameName, object parameter)
        {
            Parameters[parameName] = parameter;
        }
    }
}
