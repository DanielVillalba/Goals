-- Script for updating Goals and Objectives 
USE CDPTrack

-- Declare Variable 
	DECLARE @GoalId INT
	DECLARE @year INT
	DECLARE @month INT
	DECLARE @ExistElement INT
	DECLARE @resourceId INT
	DECLARE @Quater INT
	DECLARE @Objectiveid INT

-- Declare cursor 
DECLARE RegistryCursor CURSOR
FOR  SELECT  GoalTracking.GoalId, DATEPART(YY,GoalTracking.FinishDate) as Year, DATEPART(MM,GoalTracking.FinishDate) as Month, GoalTracking.ResourceId
	FROM GoalTracking inner join Objective ON
	GoalTracking.ObjectiveId =Objective.ObjectiveId 
	WHERE Objective.Objective like '%Unassign Objectives%'
	--AND GoalTracking.VerifiedByManager =0
	--AND  DATEPART(YY,GoalTracking.FinishDate) = 2014 ;

OPEN RegistryCursor

	FETCH NEXT FROM RegistryCursor INTO @GoalId, @year, @month,  @resourceId
	
		--Firs element and do something

	    -- check which is the Quater
		 IF(@month >=1 AND @month <= 3)
			SET @Quater= 1	
		 ELSE IF(@month >=4 AND @month <= 6)	
			SET @Quater= 2
		 ELSE IF(@month >=7 AND @month <= 9)	
			SET @Quater= 3	
		 ELSE IF(@month >=10 AND @month <= 12)	
			SET @Quater= 4	
		
		print '---- Quater----'
		print @Quater 

			-- looking  if exist
		SET @ExistElement =(
		SELECT count(*) From Objective
	    Where Objective.ResourceId = @resourceId and Quarter = @Quater and Year = @year and Objective like '%Unassign Objectives%')
		print '---- Exist----'
		print @ExistElement
		-- Compare 
		IF (@ExistElement = 0)
		BEGIN
		print 'No existe'
			
			-- Create new Objective
			INSERT INTO Objective (Objective,CategoryId,ResourceId,Year, Quarter) 
			values('Unassign Objectives',4,@resourceId,@year,@Quater);
		
			-- loking for a idObjective
			SET @Objectiveid = (SELECT ObjectiveId From Objective
								Where Objective.ResourceId = @resourceId and Quarter = @Quater and Year = @year and Objective like '%Unassign Objectives%')
			
			
			-- Move Goal 
			UPDATE GoalTracking SET ObjectiveId = @Objectiveid WHERE GoalTracking.GoalId=@GoalId
			
		END
	   ELSE
		BEGIN
	print 'Existe'
			-- loking for a idObjective
			SET @Objectiveid = (SELECT ObjectiveId From Objective
								Where Objective.ResourceId = @resourceId and Quarter = @Quater and Year = @year and Objective like '%Unassign Objectives%')

			-- Move Goal 
			UPDATE GoalTracking SET ObjectiveId = @Objectiveid WHERE GoalTracking.GoalId=@GoalId
			
		END
-----------------------------------------------------------------------------------------------------------------------------
	WHILE @@FETCH_STATUS = 0
		BEGIN
			
			FETCH NEXT FROM RegistryCursor INTO @GoalId, @year, @month, @resourceId

			-- check which is the Quater
		 IF(@month >=1 AND @month <= 3)
			SET @Quater= 1	
		 ELSE IF(@month >=4 AND @month <= 6)	
			SET @Quater= 2
		 ELSE IF(@month >=7 AND @month <= 9)	
			SET @Quater= 3	
		 ELSE IF(@month >=10 AND @month <= 12)	
			SET @Quater= 4	
				-- looking  if exist
		SET @ExistElement =(
		SELECT count(*) From Objective
	    Where Objective.ResourceId = @resourceId and Quarter = @Quater and Year = @year and Objective like '%Unassign Objectives%')
		
		-- Compare 
		IF (@ExistElement = 0)
		BEGIN;
			-- Create new Objective
			INSERT INTO Objective (Objective,CategoryId,ResourceId,Year, Quarter) 
			values('Unassign Objectives',4,@resourceId,@year,@Quater);

			-- loking for a idObjective
			SET @Objectiveid = (SELECT ObjectiveId From Objective
								Where Objective.ResourceId = @resourceId and Quarter = @Quater and Year = @year and Objective like '%Unassign Objectives%')

			-- Move Goal 
		     UPDATE GoalTracking SET ObjectiveId = @Objectiveid WHERE GoalTracking.GoalId=@GoalId
		  
		END;
	   ELSE
		BEGIN;
			-- loking for a idObjective
			SET @Objectiveid = (SELECT ObjectiveId From Objective
								Where Objective.ResourceId = @resourceId and Quarter = @Quater and Year = @year and Objective like '%Unassign Objectives%')
								print @resourceId
			

			UPDATE GoalTracking SET ObjectiveId = @Objectiveid WHERE GoalTracking.GoalId=@GoalId
			
		END;
	END;
CLOSE RegistryCursor

DEALLOCATE RegistryCursor
