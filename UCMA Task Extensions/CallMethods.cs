using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Signaling;

namespace LyncAsyncExtensionMethods
{
    public static class CallMethods
    {
        public static Task<CallMessageData> AcceptAsync(this Call call)
        {
            return Task<CallMessageData>.Factory.FromAsync(call.BeginAccept,
                call.EndAccept, null);
        }
        

        public static Task TerminateAsync(this Call call)
        {
            return Task.Factory.FromAsync(call.BeginTerminate,
                call.EndTerminate, null);
        }
       
    }
}
