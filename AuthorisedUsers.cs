using log4net;
using System;
using System.IO;
using System.Linq;

namespace IMPowershell
{
    public class AuthorisedUsers
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthorisedUsers));
        public bool UserIsAuthorised(string sipAddress)
        {
            try
            {
                if (sipAddress.StartsWith("sip:"))
                {
                    sipAddress = sipAddress.Substring(4);
                }

                var filename = IMPowershell.Properties.Settings.Default.AuthorisedUsersList;

                if (!File.Exists(filename))
                {
                    log.Warn(filename + " does not exist");
                    return false;
                }

                var fileContents = File.ReadAllText(filename);
                var fileEntries = fileContents.ToLower().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (fileEntries.Contains(sipAddress.ToLower()))
                {
                    return true;
                }              
                else
                {
                    log.WarnFormat("Message from {0} not in authorised list", sipAddress);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error in authorising user: {0}", ex);
            }
            return false;
        }
    }
}
