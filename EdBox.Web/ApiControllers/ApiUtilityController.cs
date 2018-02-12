using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.ApiControllers
{
    public class ApiUtilityController : BaseController
    {
        [HttpPost]
        public bool UploadImage()
        {
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];

                    if (file != null && file.ContentLength > 0)
                    {
                        var location = $"{HostingEnvironment.ApplicationPhysicalPath}UploadedFile";

                        if (!Directory.Exists(location))
                            Directory.CreateDirectory(location);

                        var directory = new System.IO.DirectoryInfo(location);

                        var path = $"{directory.FullName}\\{file.FileName}";
                        file.SaveAs(path);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return false;
            }
        }

        [HttpPost]
        public bool UploadDocument(int? assessmentId)
        {
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];

                    if (file != null && file.ContentLength > 0)
                    {
                        var location = $"{HostingEnvironment.ApplicationPhysicalPath}UploadedFile\\UploadedResults\\{UserInformation.UserInformationCredential.Username}\\{assessmentId}";

                        if (!Directory.Exists(location))
                            Directory.CreateDirectory(location);

                        var directory = new System.IO.DirectoryInfo(location);

                        var path = $"{directory.FullName}\\{file.FileName}";
                        file.SaveAs(path);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return false;
            }
        }

        [HttpGet]
        public bool ClearFiles()
        {
            try
            {
                var location = $"{HostingEnvironment.ApplicationPhysicalPath}UploadedFile\\UploadedResults\\{UserInformation.UserInformationCredential.Username}";
                var directory = new System.IO.DirectoryInfo(location);
                var files = directory.GetFiles();

                foreach (var file in files)
                {
                    System.IO.File.Delete(file.FullName);
                }
                return true;
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return false;
            }
        }
    }
}