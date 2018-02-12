using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Areas.Registrar.Models;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.ApiControllers.EducationManagement
{
    public class ApiClassManagementController : BaseController
    {
        [HttpGet]
        public JsonResult GetHelm()
        {
            try
            {
                using (var data = new Entities())
                {
                    var classes = data.Classes.Where(x => x.IsDeleted == false).ToList();
                    var mappedClasses = data.ClassMaps.Where(
                        x => x.IsDeleted == false && x.CredentialId == UserInformation.UserInformationCredential.Id)
                        .ToList()
                        .Select(
                            x =>
                                new
                                {
                                    x,
                                    ClassName =
                                        (classes.FirstOrDefault(y => y.Id == x.ClassId)?.ClassName ?? "No Class Name")
                                }).ToList();

                    var mappedSubject = data.SubjectMaps
                        .Where(
                            x => x.IsDeleted == false && x.CredentialId == UserInformation.UserInformationCredential.Id)
                        .ToList()
                        .Select(x =>
                            new
                            {
                                x,
                                SubjectName =
                                    (EnumDictionary.GetList<Subjects>()
                                        .FirstOrDefault(y => y.ItemId == x.SubjectId)?
                                        .ItemName ?? "No Subject Name")
                            }).ToList();


                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"All Done",
                                Data = new { Subjects = mappedSubject, Classes = mappedClasses }
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

        public JsonResult AddClassSubject(int subjectId, int classId)
        {
            try
            {
                if (subjectId == 0 || classId == 0)
                    return new JsonResult()
                    {
                        Data = new { Status = false, Message = $"Please check if your Selections are Valid" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                var credentialId = UserInformation.UserInformationCredential.Id;
                using (var data = new Entities())
                {
                    var activeSessionId = data.EducationalPeriods.FirstOrDefault(x => x.IsActive && !x.IsDeleted)?.Id ?? 0;
                    var activeSubSessionId = data.SubEducationalPeriods.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.EducationalPeriodId == activeSessionId)?.Id ?? 0;

                    if (activeSubSessionId == 0 || activeSessionId == 0)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"Please check if the Administrator has created an Active Session and an Active Sub-Session" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if (
                        data.ClassSubjects.FirstOrDefault(
                            x =>
                                x.CredentialId == credentialId && x.SessionId == activeSessionId &&
                                x.SubSessionId == activeSubSessionId && !x.IsDeleted &&
                                x.SubjectId == subjectId &&
                                x.ClassId == classId) != null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"This Class Subject has been Configured already" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.ClassSubjects.Add(new ClassSubject()
                    {
                        ClassId = classId,
                        CredentialId = credentialId,
                        DateCreated = DateTime.Now,
                        IsDeleted = false,
                        SessionId = activeSessionId,
                        SubSessionId = activeSubSessionId,
                        SubjectId = subjectId
                    });
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        public JsonResult GetClassSubjects()
        {
            try
            {
                var credentialId = UserInformation.UserInformationCredential.Id;
                using (var data = new Entities())
                {
                    var activeSession = data.EducationalPeriods.FirstOrDefault(x => x.IsActive && !x.IsDeleted);
                    if (activeSession == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        $"Please check if the Administrator has created an Active Session Active Sub-Session"
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var activeSubSession = data.SubEducationalPeriods.FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.EducationalPeriodId == activeSession.Id);
                    if (activeSubSession == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = $"Please check if the Administrator has created an Active Sub-Session"
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var listOfSubjects = EnumDictionary.GetList<Subjects>();
                    var listOfClasses = data.Classes.Where(x => x.IsDeleted == false).ToList();
                    var subjectMaps = data.SubjectMaps.Where(x => x.CredentialId == credentialId).ToList();
                    var classMaps = data.ClassMaps.Where(x => x.CredentialId == credentialId).ToList();

                    var classSubjectRaw =
                        data.ClassSubjects.Where(
                            x =>
                                !x.IsDeleted && x.SessionId == activeSession.Id && x.SubSessionId == activeSubSession.Id &&
                                x.CredentialId == credentialId)
                                .AsEnumerable()
                            .Select(
                                x =>
                                    new
                                    {
                                        x,
                                        subjectMaps.FirstOrDefault(y => y.Id == x.SubjectId)?.SubjectId,
                                        classMaps.FirstOrDefault(y => y.Id == x.ClassId)?.ClassId
                                    })
                            .ToList();

                    var classSubjects = classSubjectRaw
                        .Select(c => new
                        {
                            c.x,
                            NameOfClassSubject =
                                $"{listOfClasses.FirstOrDefault(x => x.Id == c.ClassId)?.ClassName} {listOfSubjects.FirstOrDefault(x => x.ItemId == c.SubjectId)?.ItemName}",
                            MemberResultNumber = data.ClassSubjectMemberResults.Count(x => x.ClassSubjectId == c.x.Id)
                        }).ToList();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful", Data = classSubjects },
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

        public JsonResult GetClassStudents(int currentClassId)
        {
            try
            {
                var credentialId = UserInformation.UserInformationCredential.Id;
                using (var data = new Entities())
                {
                    var currentStudentsInClass =
                        data.Students.Where(x => x.CurrentClassId == currentClassId)
                            .OrderBy(x => x.StudentSurname)
                            .ThenBy(x => x.StudentOtherName)
                            .ToList();
                    
                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful", Data = currentStudentsInClass },
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

        public JsonResult GetTodaysAttendance(int currentClassId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var date = DateTime.Now.Date;

                    var todaysAttendance =
                        data.StudentAttendances.Where(
                            x =>
                                x.ClassId == currentClassId &&
                                x.AttendanceDate == date &&
                                x.EducationalPeriodId == UserInformation.CurrentEducationalPeriod.Id &&
                                x.SubEducationalPeriodId == UserInformation.CurrentSubEducationalPeriod.Id &&
                                x.IsDeleted == false).Select(x => x.StudentId).ToList();

                    var students = data.Students.Where(x => !x.IsDeleted && x.CurrentClassId == currentClassId).ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Successful",
                                Data = new
                                {
                                    TotalAttendanceTaken = todaysAttendance.Count,
                                    AttendanceData =
                                        students.Select(
                                            x =>
                                                new
                                                {
                                                    x.StudentId,
                                                    x.StudentSurname,
                                                    x.StudentOtherName,
                                                    IsMarked = todaysAttendance.Any(y => y == x.StudentId)
                                                }).OrderBy(x => x.IsMarked).ThenBy(x => x.StudentSurname).ToList()
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

        public JsonResult GetAssessments(int classSubjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var classSubjectMemberResult =
                        data.ClassSubjectMemberResults.Where(
                            x => x.IsDeleted == false && x.ClassSubjectId == classSubjectId)
                            .ToList()
                            .Select(c => new { c, AssessmentType = ((AssessmentType)c.MemberResultType).DisplayName() })
                            .ToList();

                    var classSubject = data.ClassSubjects.FirstOrDefault(x => x.Id == classSubjectId);

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful", Data = classSubjectMemberResult, IsActivated = classSubject?.IsActivated },
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

        public JsonResult MarkAttendance(int classId, string studentId, bool presence)
        {
            try
            {
                using (var data = new Entities())
                {
                    var today = DateTime.Now.Date;

                    var attendance =
                        data.StudentAttendances.FirstOrDefault(
                            x => x.ClassId == classId &&
                                 x.AttendanceDate == today &&
                                 x.StudentId == studentId &&
                                 x.EducationalPeriodId == UserInformation.CurrentEducationalPeriod.Id &&
                                 x.SubEducationalPeriodId == UserInformation.CurrentSubEducationalPeriod.Id &&
                                 x.IsDeleted == false);

                    if (attendance == null)
                    {
                        if (presence)
                        {
                            data.StudentAttendances.Add(new StudentAttendance()
                            {
                                AttendanceDate = DateTime.Now.Date,
                                AttendanceStatus = (int) AttendanceStates.Present,
                                ClassId = classId,
                                EducationalPeriodId = UserInformation.CurrentEducationalPeriod.Id,
                                IsDeleted = false,
                                MarkedByCredentialId = UserInformation.UserInformationCredential.Id,
                                StudentId = studentId,
                                SubEducationalPeriodId = UserInformation.CurrentSubEducationalPeriod.Id
                            });
                        }
                    }
                    else
                    {
                        if (!presence)
                            data.StudentAttendances.Remove(attendance);
                    }

                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        public JsonResult AddAssessment(string assessmentName, int assessmentType, int assessmentPercentage, int classSubjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    if (data.ClassSubjectMemberResults.FirstOrDefault(x => x.MemberResultName == assessmentName && x.ClassSubjectId == classSubjectId) != null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"You already have an Assessment of this name. Please Re-Name" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var totalMembers =
                        data.ClassSubjectMemberResults.Where(
                            x => x.ClassSubjectId == classSubjectId && x.IsDeleted == false)
                            .Sum(x => x.TotalContributedMark);

                    if ((totalMembers + assessmentPercentage) > 100)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"The Marks Contributed by this Assessment will push the Total Percentage beyond 100%. Please Check" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if ((totalMembers + assessmentPercentage) == 100 &&
                        data.ClassSubjectMemberResults.FirstOrDefault(
                            x =>
                                x.ClassSubjectId == classSubjectId && x.IsDeleted == false &&
                                x.MemberResultType == (int)AssessmentType.Examination) == null &&
                        assessmentType != (int)AssessmentType.Examination)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        $"Please note that you are yet to add an Examination Assessment Type. This is Compulsory!"
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.ClassSubjectMemberResults.Add(new ClassSubjectMemberResult()
                    {
                        ClassSubjectId = classSubjectId,
                        DateConfigured = DateTime.Now,
                        IsDeleted = false,
                        MemberResultName = assessmentName,
                        TotalContributedMark = assessmentPercentage,
                        MemberResultType = assessmentType
                    });
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        public JsonResult RemoveAssessment(int assessmentId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var assessmentMember = data.ClassSubjectMemberResults.FirstOrDefault(x => x.Id == assessmentId);
                    if (assessmentMember == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"Assessment Not Found." },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    data.ClassSubjectMemberResults.Remove(assessmentMember);
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        public JsonResult ActivateClassSubject(int classSubjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var classSubject =
                        data.ClassSubjects.FirstOrDefault(x => x.IsDeleted == false && x.Id == classSubjectId);

                    if (classSubject == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"Class Subject not Found" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var totalMembers =
                        data.ClassSubjectMemberResults.Where(
                            x => x.ClassSubjectId == classSubjectId && x.IsDeleted == false)
                            .Sum(x => x.TotalContributedMark);

                    if (totalMembers != 100)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = $"The Sum of the Marks Contributed are not equal to 100%. Please Check" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if (data.ClassSubjectMemberResults.FirstOrDefault(
                        x =>
                            x.ClassSubjectId == classSubjectId && x.IsDeleted == false &&
                            x.MemberResultType == (int) AssessmentType.Examination) == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        $"Please note that you are yet to add an Examination Assessment Type. This is Compulsory!"
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    classSubject.IsActivated = true;
                    data.Entry(classSubject).State = EntityState.Modified;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        public JsonResult GetStudent(string studentId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var student =
                        data.Students.FirstOrDefault(
                            x => x.IsDeleted == false && x.StudentId == studentId);



                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Successful",
                                Data =
                                    new
                                    {
                                        StudentInformation = student,
                                        Comment =
                                            new
                                            {
                                                Teacher =
                                                    data.StudentCommentFromTeachers.FirstOrDefault(
                                                        x =>
                                                            x.StudentId == studentId &&
                                                            x.EducationalPeriod == UserInformation.CurrentEducationalPeriod.Id &&
                                                            x.SubEducationalPeriod == UserInformation.CurrentSubEducationalPeriod.Id && 
                                                            x.IsDeleted == false)?.CommentData ?? "No Comment"
                                            }
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

        public JsonResult SaveComment(string studentId, string commentData)
        {
            try
            {
                using (var data = new Entities())
                {
                    var comment =
                        data.StudentCommentFromTeachers.FirstOrDefault(
                            x =>
                                x.StudentId == studentId &&
                                x.EducationalPeriod == UserInformation.CurrentEducationalPeriod.Id &&
                                x.SubEducationalPeriod == UserInformation.CurrentSubEducationalPeriod.Id &&
                                x.IsDeleted == false);

                    if (comment == null)
                        data.StudentCommentFromTeachers.Add(new StudentCommentFromTeacher()
                        {
                            SubEducationalPeriod = UserInformation.CurrentSubEducationalPeriod.Id,
                            CommentData = commentData,
                            EducationalPeriod = UserInformation.CurrentSubEducationalPeriod.Id,
                            IsDeleted = false,
                            MarkedByCredentialId = UserInformation.UserInformationCredential.Id,
                            StudentId = studentId
                        });
                    else
                    {
                        comment.CommentData = commentData;
                        data.Entry(comment).State = EntityState.Modified;
                    }

                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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