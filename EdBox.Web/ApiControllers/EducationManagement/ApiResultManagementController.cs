using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CsvHelper.Configuration;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Core.FileSchema;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.ApiControllers.EducationManagement
{
    public class ApiResultManagementController : BaseController
    {
        // GET: ApiResultManagement
        public JsonResult GetHelm()
        {
            try
            {
                using (var data = new Entities())
                {
                    var classes = data.Classes.Where(x => x.IsDeleted == false).ToList();
                    var subjects = EnumDictionary.GetList<Subjects>();
                    var classMaps =
                        data.ClassMaps.Where(x => x.CredentialId == UserInformation.UserInformationCredential.Id && x.IsDeleted == false)
                            .ToList();
                    var subjectMaps =
                        data.SubjectMaps.Where(x => x.CredentialId == UserInformation.UserInformationCredential.Id && x.IsDeleted == false)
                            .ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Success",
                                Data =
                                    new
                                    {
                                        Assessments =
                                            data.Assessments.Where(x => x.IsDeleted == false)
                                                .OrderBy(x => x.AssessmentName)
                                                .ToList(),
                                        Classes = classMaps
                                            .ToList()
                                            .Select(
                                                x =>
                                                    new
                                                    {
                                                        x,
                                                        ClassName =
                                                            classes.FirstOrDefault(y => y.Id == x.ClassId)?
                                                                .ClassName ?? ""
                                                    }),
                                        Subjects = subjectMaps
                                            .ToList()
                                            .Select(
                                                x =>
                                                    new
                                                    {
                                                        x,
                                                        SubjectName =
                                                            subjects.FirstOrDefault(y => y.ItemId == x.SubjectId)?
                                                                .ItemName ?? ""
                                                    })
                                    }
                            },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        public JsonResult GetUploadHistory(int assessmentId, int classId, int subjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var uploadHistory =
                        data.UploadHistories.Where(
                            x =>
                                x.CredentialId == UserInformation.UserInformationCredential.Id && x.IsDeleted == false &&
                                x.AssessmentId == assessmentId &&
                                x.SubjectId == subjectId &&
                                x.ClassId == classId)
                            .OrderBy(x => x.DateProcessEnd)
                            .ToList()
                            .Select(u => new
                            {
                                u,
                                DateProcessStart = u.DateProcessStart.ToLongDateString(),
                                DateProcessEnd = u.DateProcessEnd.ToLongDateString(),
                                DateUploaded = u.DateUploaded.ToLongDateString(),
                                ProcessingStatus = ((FileProcessingState) u.ProcessingStatus).DisplayName()
                            })
                            .ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Success",
                                Data = uploadHistory
                            },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        public JsonResult ProcessUploadedFiles(int assessmentId, IEnumerable<string> fileNames, int classId, int subjectId)
        {
            try
            {
                var location = $"{HostingEnvironment.ApplicationPhysicalPath}UploadedFile\\UploadedResults\\{UserInformation.UserInformationCredential.Username}\\{assessmentId}";

                if (!Directory.Exists(location))
                    Directory.CreateDirectory(location);
                
                var directory = new System.IO.DirectoryInfo(location);
                var userId = UserInformation.UserInformationCredential.Id;
                var status = Newtonsoft.Json.JsonConvert.SerializeObject(new OperationMessages()
                {
                    FileProcessingState = FileProcessingState.New,
                    Message = "File Received",
                    FileProcessingMessageTypes = FileProcessingMessageTypes.Information
                });

                using (var data = new Entities())
                {
                    var currentSessionId = data.EducationalPeriods.FirstOrDefault(x => x.IsActive)?.Id;
                    var currentSemesterId = data.SubEducationalPeriods.FirstOrDefault(x => x.IsActive)?.Id;

                    foreach (var file in fileNames)
                    {
                        var path = $"{directory.FullName}\\{file}";
                        var fileInfo = new FileInfo(path);

                        var history = data.UploadHistories.Add(new UploadHistory()
                        {
                            AssessmentId = assessmentId,
                            CredentialId = userId,
                            DateProcessEnd = DateTime.Now,
                            DateProcessStart = DateTime.Now,
                            DateUploaded = DateTime.Now,
                            FileSize = fileInfo.Length,
                            IsDeleted = false,
                            OperationMessages = $"{status}|",
                            ProcessingStatus = (int) FileProcessingState.New,
                            RawFileName = file,
                            SystemFileName = $"{Guid.NewGuid().ToString()}.{fileInfo.Extension}",
                            ClassId = classId,
                            SubjectId = subjectId
                        });
                        data.SaveChanges();

                        var historyId = history.Id;

                        history.OperationMessages += Newtonsoft.Json.JsonConvert.SerializeObject(new OperationMessages()
                        {
                            FileProcessingState = FileProcessingState.Processing,
                            Message = "Processing Started",
                            FileProcessingMessageTypes = FileProcessingMessageTypes.Information
                        }) + "|";
                        data.SaveChanges();

                        new Thread(() =>
                        {
                            var configuration = new CsvConfiguration
                            {
                                HasHeaderRecord = true,
                                BufferSize = 1024,
                                SkipEmptyRecords = true,
                                IsHeaderCaseSensitive = false
                            };
                            configuration.RegisterClassMap<StudentAssessmentResultMap>();

                            var splitRecords =
                                BulkUpload.GetFileDataToList<StudentAssessmentResult>(path, configuration)
                                    .Select((x, i) => new {Index = i, Value = x})
                                    .GroupBy(x => x.Index/500)
                                    .Select(x => x.Select(v => v.Value).ToList())
                                    .ToList();

                            Parallel.ForEach(splitRecords, (splitRecord, parallellLoopState, i) =>
                            {
                                var currentSessionIdInt = Convert.ToInt32(currentSessionId);
                                var currentSemesterIdInt = Convert.ToInt32(currentSemesterId);

                                using (var innerData = new Entities())
                                {
                                    foreach (var record in splitRecord)
                                    {
                                        //remove existing
                                        var resultRaw = innerData.ResultRaws.FirstOrDefault(x => x.IsDeleted == false &&
                                                                                                 x.AssessmentId == assessmentId &&
                                                                                                 x.SessionId == currentSessionIdInt &&
                                                                                                 x.SubSessionId == currentSemesterIdInt &&
                                                                                                 x.ClassId == classId &&
                                                                                                 x.SubjectId == subjectId &&
                                                                                                 x.StudentId == record.StudentId);

                                        if (resultRaw != null)
                                        {
                                            innerData.ResultRaws.Remove(resultRaw);
                                            innerData.SaveChanges();
                                        }

                                        innerData.ResultRaws.Add(new ResultRaw()
                                        {
                                            AssessmentId = assessmentId,
                                            DateUploaded = DateTime.Now,
                                            FileId = historyId,
                                            IsDeleted = false,
                                            ScoreObtained = Convert.ToDecimal(record.MarksObtained.Trim()),
                                            ScoreTotal = Convert.ToDecimal(record.MarksObtainable.Trim()),
                                            SessionId = Convert.ToInt32(currentSessionId),
                                            StudentId = record.StudentId,
                                            SubSessionId = Convert.ToInt32(currentSemesterId),
                                            ClassId = classId,
                                            SubjectId = subjectId,
                                            CredentialId = userId
                                        });
                                        innerData.SaveChanges();
                                    }
                                }
                            });

                            using (var innerData = new Entities())
                            {
                                var innerHistory = innerData.UploadHistories.FirstOrDefault(x => x.Id == historyId);
                                innerHistory.OperationMessages += Newtonsoft.Json.JsonConvert.SerializeObject(new OperationMessages
                                    ()
                                {
                                    FileProcessingState = FileProcessingState.Completed,
                                    Message = "Processing Completed",
                                    FileProcessingMessageTypes = FileProcessingMessageTypes.Information
                                }) + "|";
                                innerHistory.DateProcessEnd = DateTime.Now;
                                innerHistory.ProcessingStatus = (int) FileProcessingState.Completed;
                                innerData.SaveChanges();
                            }

                            System.IO.File.Delete(path);
                        }).Start();
                    }
                }

                return new JsonResult()
                {
                    Data =
                        new
                        {
                            Status = true,
                            Message = $"Succesfully Queued for Processing",
                            Data = ""
                        },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        public JsonResult GetUploadedResults(int assessmentId, int classId, int subjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var uploadedData =
                        data.ResultRaws.Where(
                            x => x.AssessmentId == assessmentId && x.ClassId == classId && x.SubjectId == subjectId)
                            .OrderBy(x => x.StudentId)
                            .ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Success",
                                Data = uploadedData
                            },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}