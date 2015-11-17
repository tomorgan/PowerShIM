using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;

namespace LyncAsyncExtensionMethods
{
    public static class CollaborationPlatformMethods
    {
     
        public static Task StartupAsync(this CollaborationPlatform platform)
        {
            return Task.Factory.FromAsync(platform.BeginStartup,
                platform.EndStartup, null);
        }

        public static Task ShutdownAsync
            (this CollaborationPlatform platform)
        {
            return Task.Factory.FromAsync(platform.BeginShutdown,
                platform.EndShutdown, null);
        }
    }
}
