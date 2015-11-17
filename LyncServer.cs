using log4net;
using LyncAsyncExtensionMethods;
using Microsoft.Rtc.Collaboration;
using System;
using System.Threading.Tasks;

namespace IMPowershell
{
    public class LyncServer
    {
        private string _appUserAgent = "IM Powershell";
        private string _appID = "IMPowershell";
        private CollaborationPlatform _collabPlatform;
        private ApplicationEndpoint _endpoint;
    

        public event EventHandler<EventArgs> LyncServerReady = delegate { };
        public event EventHandler<CallReceivedEventArgs<InstantMessagingCall>> IncomingCall = delegate { };

        private static readonly ILog log = LogManager.GetLogger(typeof(LyncServer));

        public async Task Start()
        {
            try
            {
                log.Info("Starting Collaboration Platform");
                ProvisionedApplicationPlatformSettings settings = new ProvisionedApplicationPlatformSettings(_appUserAgent, _appID);
           
                _collabPlatform = new CollaborationPlatform(settings);
                _collabPlatform.RegisterForApplicationEndpointSettings(OnNewApplicationEndpointDiscovered);                
                
                await _collabPlatform.StartupAsync();

                log.Info("Platform Started");
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error establishing collaboration platform: {0}", ex.ToString());
            }
        }

        public async Task Stop()
        {
            log.Info("Stopping Lync Server");
            await _endpoint.TerminateAsync();
            await _collabPlatform.ShutdownAsync();
        }

     

        private async void OnNewApplicationEndpointDiscovered(object sender, ApplicationEndpointSettingsDiscoveredEventArgs e)
        {
            log.Info(string.Format("New Endpoint {0} discovered", e.ApplicationEndpointSettings.OwnerUri));
            _endpoint = new ApplicationEndpoint(_collabPlatform, e.ApplicationEndpointSettings);
            _endpoint.RegisterForIncomingCall<InstantMessagingCall>(OnIncomingIM);
            
            await _endpoint.EstablishAsync();
            log.Info("Endpoint established");
           LyncServerReady(this, new EventArgs());
        }

        //new incoming instant message conversation
        private void OnIncomingIM(object sender, CallReceivedEventArgs<InstantMessagingCall> e)
        {
            log.Info(string.Format("New Message from {0} : {1}", e.Call.RemoteEndpoint.Uri, e.ToastMessage.Message));
            IncomingCall(this, e);
        }

      


    }
}
