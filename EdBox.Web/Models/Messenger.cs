using System.Linq;
using EdBox.Core;

namespace EdBox.Web.Models
{
    public static class Messenger
    {
        public static bool SendNewCredentials(string username)
        {
            using (var data = new Entities())
            {
                var user = data.Credentials.FirstOrDefault(x => x.Username == username);
                if (user == null)
                    return false;

                var message = $"Welcome to EdBox {user.LastName}, {user.FirstName}. <br/>";
                message += $"Username: {username} <br/>";
                message += $"Password: {Encryption.SaltDecrypt(user.PasswordData, user.PasswordSalt)}<br/>";
                message += $"Best Regards";

                return Messaging.SendMail(username, "", "support@codesistance.com", "Registration", message, "");
            }
        }

        public static bool SendCredentials(string username)
        {
            using (var data = new Entities())
            {
                var user = data.Credentials.FirstOrDefault(x => x.Username == username);
                if (user == null)
                    return false;

                var message = $"Hello {user.LastName}, {user.FirstName}. You have requested that your Credentials be resent. Find them below.<br/>";
                message += $"Username: {username} <br/>";
                message += $"Password: {Encryption.SaltDecrypt(user.PasswordData, user.PasswordSalt)}<br/>";
                message += $"Best Regards";

                return Messaging.SendMail(username, "", "support@codesistance.com", "Credential Request", message, "");
            }
        }

        public static bool SendStudentEntry(string username, string studentId)
        {
            return true;
        }
    }
}
