using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EdBox.Core;
using EdBox.Core.EnumLib;
using Microsoft.SqlServer.Server;

namespace EdBox.Web.Models
{
    public static class UserInformation
    {
        public static void ActivateSession(Credential userData)
        {
            HttpContext.Current.Session["UserData"] = userData;
            var data = new Entities();

            CurrentEducationalPeriod = data.EducationalPeriods.FirstOrDefault(x => x.IsActive && !x.IsDeleted);

            CurrentSubEducationalPeriod =
                data.SubEducationalPeriods.FirstOrDefault(
                    x => x.IsActive && !x.IsDeleted && x.EducationalPeriodId == CurrentEducationalPeriod.Id);

            if (userData.Id == 0)
            {
                Route = UserRoles.Administration.DisplayName();
                Role = new CredentialMap()
                {
                    Id = 0,
                    IsDeleted = false,
                    CredentialId = 0, 
                    RoleId = (int)UserRoles.Administration,
                    DateAssigned = DateTime.Now
                };
                RoleName = "System Administrator";
                HasClassToManage = false;
                HasStudentsToManage = false;
            }
            else
            {
                Role = data.CredentialMaps.FirstOrDefault(x => x.CredentialId == userData.Id);
                Route = ((UserRoles)Role.RoleId).ToString();
                RoleName = ((UserRoles)Role.RoleId).DisplayName();
                HasClassToManage = data.ClassMaps.Any(x => x.CredentialId == userData.Id && x.IsDeleted == false);
                HasStudentsToManage = data.Students.Any(x => x.AuuthCredentialId == userData.Id && x.IsDeleted == false);

                if (HasStudentsToManage)
                {
                    StudentsList =
                        data.Students.Where(x => x.PGEmail == userData.Username && x.IsDeleted == false).ToList();
                }
            }
            data.Dispose();
        }

        public static bool IsSessionValid => HttpContext.Current.Session["UserData"] != null;

        public static Credential UserInformationCredential => ((Credential)HttpContext.Current.Session["UserData"]);

        public static void DeactivateSession()
        {
            HttpContext.Current.Session["UserData"] = null;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
        }

        public static string Route { get; private set; }

        public static string RoleName { get; private set; }

        public static bool HasClassToManage { get; private set; }

        public static bool HasStudentsToManage { get; private set; }

        public static CredentialMap Role { get; private set; }

        public static List<Student> StudentsList { get; private set; }

        public static EducationalPeriod CurrentEducationalPeriod { get; private set; }

        public static SubEducationalPeriod CurrentSubEducationalPeriod { get; private set; }

        public static PinState PinRegResultView(string studentId)
        {
            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["PinRegResultView"] ?? "false") == false)
                    return PinState.NoPinRequired;

                using (var data = new Entities())
                {
                    var pinBatch = data.PinBatches.FirstOrDefault(x =>
                        x.IsDeleted == false &&
                        x.EducationalPeriod == CurrentEducationalPeriod.Id
                        && x.SubEducationalPeriod == CurrentSubEducationalPeriod.Id);

                    if (pinBatch == null)
                        return PinState.PinRequiredNotFound;

                    var pin = data.PinBatchMembers.FirstOrDefault(x =>
                        x.IsDeleted == false && x.StudentId == studentId && x.BatchId == pinBatch.Id);

                    return pin == null ? PinState.PinRequiredNotFound : PinState.PinRequiredFound;
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return PinState.NoPinRequired;
            }
        }
    }
}
