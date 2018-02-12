using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Areas.Administration.Models;
using EdBox.Web.Controllers;
using EdBox.Core;
using EdBox.Core.EnumLib;

namespace EdBox.Web.ApiControllers.Administration
{
    public class ApiConfigurationController : BaseController
    {
        [HttpGet]
        public JsonResult GetClasses()
        {
            try
            {
                using (var data = new Entities())
                {
                    var classes = data.Classes.Where(x => x.IsDeleted == false).ToList();
                    var classMaps = data.ClassMaps.Where(x => x.IsDeleted == false).ToList();
                    var students = data.Students.Where(x => x.IsDeleted == false).ToList();

                    var resolvedClasses =
                        classes.Select(c => new
                        {
                            c,
                            FacilitatorMappingCount = classMaps.Count(x => x.ClassId == c.Id),
                            StudentMappingCount = students.Count(x => x.CurrentClassId == c.Id)
                        }).OrderBy(x => x.c.ClassName).ToList();

                    return new JsonResult()
                    {
                        Data =
                            new {Status = true, Message = $"Loaded {resolvedClasses.Count()}", Data = resolvedClasses},
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new {Status = false, Message = ex.Message, Data = string.Empty},
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpGet]
        public JsonResult AddClass(string className)
        {
            try
            {
                using (var data = new Entities())
                {
                    var classInfo = data.Classes.FirstOrDefault(x => x.IsDeleted == false && x.ClassName == className);

                    if (classInfo != null)
                        return new JsonResult()
                        {
                            Data =
                            new { Status = false, Message = $"Class Name already exists. Please try another"},
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.Classes.Add(new Class() {ClassName = className.ToUpper(), DateCreated = DateTime.Now, IsDeleted = false});
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Successful" },
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
        public JsonResult DeleteClass(int classId)
        {
            try
            {
                using (var data = new Entities())
                {
                    if (data.ClassMaps.FirstOrDefault(x => x.IsDeleted == false && x.ClassId == classId) != null)
                        return new JsonResult()
                        {
                            Data =
                            new { Status = true, Message = $"Cannot Remove Class as it it still attached to a Facilitator" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if (data.Students.FirstOrDefault(x => x.IsDeleted == false && x.CurrentClassId == classId) != null)
                        return new JsonResult()
                        {
                            Data =
                            new { Status = true, Message = $"Cannot Remove Class as it it still attached to a Student" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.Classes.Remove(data.Classes.FirstOrDefault(x => x.IsDeleted == false && x.Id == classId));
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Successful" },
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
        public JsonResult GetSessions()
        {
            try
            {
                using (var data = new Entities())
                {
                    var eduPeriodRaw = data.EducationalPeriods.Where(x => x.IsDeleted == false).ToList();

                    var educationalPeriod =
                        eduPeriodRaw.Select(e => new
                        {
                            e.PeriodName,
                            PeriodStart = e.PeriodStart.ToLongDateString(),
                            PeriodEnd = e.PeriodEnd.ToLongDateString(),
                            e.IsActive,
                            e.Id
                        }).OrderByDescending(x => x.IsActive).ThenByDescending(x => x.PeriodEnd).ToList();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Loaded", Data = educationalPeriod },
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
        public JsonResult ActivateSession(int sessionId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var existingEduPeriod = data.EducationalPeriods.FirstOrDefault(x => x.IsActive == true);
                    if (existingEduPeriod != null)
                    {
                        existingEduPeriod.IsActive = false;
                        existingEduPeriod.PeriodEnd = DateTime.Now;
                        data.SaveChanges();
                    }

                    var educationalPeriod = data.EducationalPeriods.FirstOrDefault(x => x.IsDeleted == false && x.Id == sessionId);
                    if (educationalPeriod == null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"Educational Period NOT Found", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    educationalPeriod.IsActive = true;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Successful", Data = educationalPeriod },
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
        public JsonResult AddSession(string sessionName)
        {
            try
            {
                using (var data = new Entities())
                {
                    var educationalPeriod = data.EducationalPeriods.FirstOrDefault(x => x.IsDeleted == false && x.PeriodName == sessionName);

                    if (educationalPeriod != null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"Educational Period has been configured Already", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.EducationalPeriods.Add(new EducationalPeriod()
                    {
                        IsActive = false,
                        PeriodStart = DateTime.Now, 
                        PeriodEnd = DateTime.Now, 
                        IsDeleted = false,
                        PeriodName = sessionName
                    });
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Successful", Data = "" },
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
        public JsonResult GetSubSessions()
        {
            try
            {
                using (var data = new Entities())
                {
                    var educationalPeriod =
                        data.EducationalPeriods.FirstOrDefault(x => x.IsDeleted == false && x.IsActive == true);

                    if (educationalPeriod == null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"No Active Educational Period has been configured.", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var educationalSubPeriod =
                        data.SubEducationalPeriods.Where(x => x.EducationalPeriodId == educationalPeriod.Id)
                            .ToList()
                            .Select(e => new
                            {
                                e.SubPeriodName,
                                PeriodStart = e.PeriodStart.ToLongDateString(),
                                PeriodEnd = e.PeriodEnd.ToLongDateString(),
                                e.IsActive,
                                e.Id,
                                e.SubPeriodOrder
                            }).OrderByDescending(x => x.IsActive).ThenBy(x => x.SubPeriodOrder).ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Loaded {educationalSubPeriod.Count} Sub Periods",
                                Data = educationalSubPeriod
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

        [HttpGet]
        public JsonResult ActivateSubSession(int subSessionId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var educationalPeriod =
                        data.EducationalPeriods.FirstOrDefault(x => x.IsDeleted == false && x.IsActive == true);

                    if (educationalPeriod == null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"No Active Educational Period has been configured.", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var existingEduSubPeriod =
                        data.SubEducationalPeriods.FirstOrDefault(
                            x => x.IsActive == true && x.EducationalPeriodId == educationalPeriod.Id);

                    if (existingEduSubPeriod != null)
                    {
                        existingEduSubPeriod.IsActive = false;
                        existingEduSubPeriod.PeriodEnd = DateTime.Now;
                        data.SaveChanges();
                    }

                    var subEducationalPeriod = data.SubEducationalPeriods.FirstOrDefault(x => x.Id == subSessionId);
                    if (subEducationalPeriod == null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"Invalid Sub Educational Period selected.", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    subEducationalPeriod.IsActive = true;
                    data.Entry(subEducationalPeriod).State = EntityState.Modified;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Successful", Data = subEducationalPeriod },
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
        public JsonResult AddSubSession(string subSessionName, int subSessionOrder)
        {
            try
            {
                using (var data = new Entities())
                {
                    var educationalPeriod =
                        data.EducationalPeriods.FirstOrDefault(x => x.IsDeleted == false && x.IsActive == true);

                    if (educationalPeriod == null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"No Active Educational Period has been configured.", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if(data.SubEducationalPeriods.FirstOrDefault(x => x.EducationalPeriodId == educationalPeriod.Id && x.SubPeriodOrder == subSessionOrder) != null)
                        return new JsonResult()
                        {
                            Data =
                                new { Status = false, Message = $"A Sub Educational Period already has the specifiedNominal. Please enter another.", Data = "" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.SubEducationalPeriods.Add(new SubEducationalPeriod()
                    {
                        EducationalPeriodId = educationalPeriod.Id, 
                        IsActive = false, 
                        IsDeleted = false,
                        PeriodEnd = DateTime.Now,
                        PeriodStart = DateTime.Now,
                        SubPeriodName = subSessionName, 
                        SubPeriodOrder = subSessionOrder
                    });

                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new { Status = true, Message = $"Successful", Data = "" },
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
        public JsonResult AddAssessment(string assessmentName, double percentage, int assessmentType)
        {
            try
            {
                using (var data = new Entities())
                {
                    var assessment = data.Assessments.FirstOrDefault(x => x.IsDeleted == false && x.AssessmentName == assessmentName);
                    if (assessment != null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "This Assessment has been added before. Please Confirm", Data = string.Empty },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    
                    var assessmentsSum = 0.0m;
                    if (data.Assessments.Any(x => x.IsDeleted == false))
                    {
                        assessmentsSum = data.Assessments.Where(x => x.IsDeleted == false).Sum(x => x.AssmentPercentage);
                    }

                    if (
                        !data.Assessments.Any(
                            x => x.IsDeleted == false && x.AssessmentType == (int) AssessmentType.Examination) &&
                        (assessmentType != (int) AssessmentType.Examination) &&
                        (assessmentsSum + Convert.ToDecimal(percentage) == 100.0m))
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "You cannot add any more Continous Assessments at this Point. Please add an Examination", Data = string.Empty },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if ((assessmentsSum + Convert.ToDecimal(percentage)) > 100.0m)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = "The Cummulative Percentage is Greater than 100% Please Check",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.Assessments.Add(new Assessment()
                    {
                        AssessmentName = assessmentName,
                        AssessmentType = assessmentType,
                        AssmentPercentage = Convert.ToDecimal(percentage),
                        IsDeleted = false
                    });
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = "Successful", Data = string.Empty },
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
        public JsonResult GetAssessments()
        {
            try
            {
                using (var data = new Entities())
                {
                    var assessment = data.Assessments.Where(x => x.IsDeleted == false)
                        .ToList()
                        .Select(a => new {a, AssessmentTypeDisplay = ((AssessmentType) a.AssessmentType).DisplayName()})
                        .ToList();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = "Successful", Data = assessment },
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
        public JsonResult RemoveAssessment(int assessmentId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var sessionId = data.EducationalPeriods.FirstOrDefault(x => x.IsActive)?.Id;
                    var subSessionId =
                        data.SubEducationalPeriods.FirstOrDefault(x => x.IsActive && x.EducationalPeriodId == sessionId)
                            ?.Id;

                    if (data.ResultRaws.Any(
                        x =>
                            x.AssessmentId == assessmentId && x.SessionId == sessionId &&
                            x.SubSessionId == subSessionId && x.IsDeleted == false))
                    {
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = "This Assessment already has some data against it. It cant be removed",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }

                    var assessment = data.Assessments.FirstOrDefault(x => x.Id == assessmentId);
                    assessment.IsDeleted = true;
                    data.Entry(assessment).State = EntityState.Modified;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = "Successful",
                                Data = string.Empty
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