using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.Presence;
using Microsoft.Rtc.Signaling;

namespace LyncAsyncExtensionMethods
{
    public static class LocalEndpointMethods
    {
        public static Task<SipResponseData> EstablishAsync(this 
            LocalEndpoint endpoint)
        {
            return Task<SipResponseData>.Factory.FromAsync(
                endpoint.BeginEstablish,
                endpoint.EndEstablish, null);
        }

      

      

        public static Task TerminateAsync(this LocalEndpoint endpoint)
        {
            return Task.Factory.FromAsync(endpoint.BeginTerminate,
                endpoint.EndTerminate, null);
        }
    }
}
