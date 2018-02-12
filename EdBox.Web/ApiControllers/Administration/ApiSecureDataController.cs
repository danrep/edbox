using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.ApiControllers.Administration
{
    public class ApiSecureDataController : BaseController
    {
        [HttpGet]
        public JsonResult GetBatchItems(int batchId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var batchItems =
                        data.PinBatchMembers.Where(x => x.IsDeleted == false && x.BatchId == batchId)
                            .ToList()
                            .Select(x => new {DateUsed = x.DateUsed.ToLongDateString(), x});

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful. {batchItems} PINS Found in this Batch", Data = batchItems },
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

        [HttpGet]
        public JsonResult CreateBatchItems(int batchSpace)
        {
            try
            {
                if (batchSpace > 1000)
                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = false,
                                Message = $"Sorry. A Single Batch cannot have more than 1000 members. Please reduce the Batch Space",
                                Data = string.Empty
                            },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                using (var data = new Entities())
                {
                    var batch = new PinBatch()
                    {
                        BatchDate = DateTime.Now, 
                        BatchSpace = batchSpace, 
                        EducationalPeriod = UserInformation.CurrentEducationalPeriod.Id,
                        SubEducationalPeriod = UserInformation.CurrentSubEducationalPeriod.Id, 
                        IsDeleted = false
                    };

                    data.PinBatches.Add(batch);
                    data.SaveChanges();

                    var pinBatch = batch.Id;

                    new Thread(() =>
                    {
                        using (var innerData = new Entities())
                        {
                            for (var i = 1; i <= batchSpace; i++)
                            {
                                innerData.PinBatchMembers.Add(new PinBatchMember()
                                {
                                    BatchId = pinBatch,
                                    CredentialId = 0,
                                    DateUsed = DateTime.Now,
                                    IsUsed = false,
                                    IsDeleted = false,
                                    PinData = DateTime.Now.AddMilliseconds(i).AddTicks(i).Ticks.ToString(),
                                    StudentId = ""
                                });
                            }
                            innerData.SaveChanges();
                        }
                    }).Start();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful", Data = string.Empty },
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