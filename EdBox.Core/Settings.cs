using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Core
{
    public static class Setting
    {
        public static dynamic MailSettings()
        {
            return new
            {
                SmtpMailFrom = ConfigurationManager.AppSettings["LogRollOver"] ?? "info@EdBox.com",
                SmtpMailHead = ConfigurationManager.AppSettings["SmtpMailHead"] ?? "EdBox Messaging",
                SmtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com",
                SmtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "notification@codesistance.com",
                SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "skyRunn3r",
                SmtpSslMode = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSslMode"] ?? "true"),
                SmtpServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpServerPort"] ?? "587")
            };
        }

        public static string LogFolder => ConfigurationManager.AppSettings["LogFolder"] ?? "\\";
        public static long LogRollOver => Convert.ToInt64(ConfigurationManager.AppSettings["LogRollOver"] ?? "100000");
    }
}
