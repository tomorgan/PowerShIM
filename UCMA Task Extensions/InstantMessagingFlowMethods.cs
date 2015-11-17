using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;

namespace LyncAsyncExtensionMethods
{
    public static class InstantMessagingFlowMethods
    {
      

        public static Task<SendInstantMessageResult> SendInstantMessageAsync(this 
            InstantMessagingFlow flow, string textBody)
        {
            return Task<SendInstantMessageResult>.Factory.FromAsync(flow.BeginSendInstantMessage,
                flow.EndSendInstantMessage, textBody, null);
        }

    

      
    }
}
