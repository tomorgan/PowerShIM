using Microsoft.Rtc.Collaboration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using LyncAsyncExtensionMethods;
using log4net;

namespace IMPowershell
{
    public class PSRunner
    {
        private InstantMessagingCall _call { get; set; }
        private string _commandString { get; set; }
        private string _user { get; set; }

        private static readonly ILog log = LogManager.GetLogger(typeof(PSRunner));

        public PSRunner(InstantMessagingCall call, string toast)
        {
            try
            {
                _call = call;
                _user = call.RemoteEndpoint.Participant.Uri;
                _commandString = toast;

            }
            catch (Exception ex)
            {
               log.ErrorFormat("Error instantiating PowerShell Runner: {0}",ex);
            }
        }

        public async void ParseAndExecute()
        {
            try
            {
                await _call.AcceptAsync();
                if (UserIsAuthorised(_user))
                {
                    await TryRunPowershell(_commandString);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error parsing/executing PowerShell: {0}", ex);
            }
        }

        private async Task TryRunPowershell(string _commandString)
        {           
                await SendIM("Executing...");
                var output = RunPowershell(_commandString);
                await SendIM(output);
                await SendIM("Finished.");
                await TerminateCall();           
        }

        private async Task TerminateCall()
        {
            if (_call != null && _call.State == CallState.Established)
            {
                await _call.TerminateAsync();
            }
        }

        private async Task<SendInstantMessageResult> SendIM(string v)
        {
            return await _call.Flow.SendInstantMessageAsync(v);
        }  

        private string RunPowershell(string _commandString)
        {
            try
            {
                Collection<PSObject> results;
                using (Runspace runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.Open();

                    // create a pipeline and feed it the script text 
                    Pipeline pipeline = runspace.CreatePipeline();

                    //pre-command actions
                    pipeline.Commands.Add("Import-Module");
                    var command = pipeline.Commands[0];
                    command.Parameters.Add("Name", @"lync");

                    pipeline.Commands.AddScript(_commandString);

                    //post-command actions
                    pipeline.Commands.Add("Out-String");

                    results = pipeline.Invoke();
                    runspace.Close();
                }

                // convert the script result into a single string 
                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }
                            
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error executing PowerShell: {0}", ex);
                return "Error: " + ex.Message;
            }
        }

        private bool UserIsAuthorised(string _user)
        {
            var authCheck = new AuthorisedUsers();
            return authCheck.UserIsAuthorised(_user);
        }
    }
}
