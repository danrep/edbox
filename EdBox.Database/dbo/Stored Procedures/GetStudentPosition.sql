CREATE procedure [dbo].[GetStudentPosition]
	@studentId varchar(100),
	@classId int,
	@isSession bit
as
begin
	declare @students table 
	(
		Id int Primary Key IDENTITY(1,1),
		StudentId varchar(100) not null
	)

	declare @studentResultProvisional table 
	(
		StudentId varchar(100) not null, 
		Score decimal(18, 5)
	)

	declare @studentResultOrdered table 
	(
		Id int Primary Key IDENTITY(1,1),
		StudentId varchar(100) not null, 
		Score decimal(18, 5)
	)

	insert @students 
	select StudentId from Student where CurrentClassId = @classId and IsDeleted = 'false'

	declare @currentId int 
	set @currentId = 1
	
	declare @currentSessionId int
	declare @currentSubSessionId int
	declare @totalScore decimal(18,5)
	declare @currentStudentId varchar(100)
	
	select @currentSessionId = Id from EducationalPeriod where IsDeleted = 'false' and IsActive = 'true'
	select @currentSubSessionId = Id from SubEducationalPeriod where IsDeleted = 'false' and IsActive = 'true' and EducationalPeriodId = @currentSessionId

	while (@currentId <= (select max(Id) from @students))
    begin
		select @currentStudentId = StudentId from @students where Id = @currentId

		if(@isSession = 'true')
		begin
			select @totalScore = sum(ScoreObtained) from ResultRaw
			where IsDeleted = 'false' and StudentId = @currentStudentId and SessionId = @currentSessionId 
			and ClassId = @classId 
		end
		else
		begin
			select @totalScore = sum(ScoreObtained) from ResultRaw
			where IsDeleted = 'false' and StudentId = @currentStudentId and SessionId = @currentSessionId and SubSessionId = @currentSubSessionId
			and ClassId = @classId 
		end

		insert into @studentResultProvisional(StudentId, Score)
		select @currentStudentId, @totalScore

		set @currentId = @currentId + 1
    end

	insert @studentResultOrdered
	select StudentId, Score from @studentResultProvisional order by Score desc

	select * from @studentResultOrdered where StudentId = @studentId
end