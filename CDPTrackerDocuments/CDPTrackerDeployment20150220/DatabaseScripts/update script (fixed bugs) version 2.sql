-- Script for assigning a Quarter to Objecives that have Quarter NULL

use CDPTrack

-- Declare variables for use on Cursor 
DECLARE @ObjectiveId INT
DECLARE @contain INT
DECLARE @coun INT
DECLARE @date INT
DECLARE @Quater INT

SET @coun = 0
SET @contain=0

DECLARE RegistryCursor CURSOR

FOR  SELECT  ObjectiveId FROM Objective where Quarter IS NULL

OPEN RegistryCursor
	FETCH NEXT FROM RegistryCursor INTO @ObjectiveId
	--Firs element and do something
		SET @contain = (SELECT COUNT(*) FROM GoalTracking WHERE ObjectiveId = @ObjectiveId);

		IF(@contain != 0)
		BEGIN
		   SET @date = (SELECT MIN(DATEPART(MM,GoalTracking.FinishDate)) FROM GoalTracking where ObjectiveId = @ObjectiveId);

		 -- check which is the Quater
		 IF(@date >=1 AND @date <= 3)
			SET @Quater= 1	
		 ELSE IF(@date >=4 AND @date <= 6)	
			SET @Quater= 2
		 ELSE IF(@date >=7 AND @date <= 9)	
			SET @Quater= 3	
		 ELSE IF(@date >=10 AND @date <= 12)	
			SET @Quater= 4	

		 -- UPDATE Objective with a new Quarter 
		UPDATE Objective SET Quarter = @Quater where ObjectiveId = @ObjectiveId

		END
		ELSE 
		BEGIN
		-- IF Objective doesn't have Goals, the Quarter is one
			UPDATE Objective SET Quarter = 1 where ObjectiveId = @ObjectiveId
		END

	-- End of First Element
------------------------------------------- anothers elements in the consulting---------------------------------------------------------------------------------
WHILE @@FETCH_STATUS = 0
  BEGIN	
    FETCH NEXT FROM RegistryCursor INTO @ObjectiveId
      
	  SET @contain = (SELECT COUNT(*) FROM GoalTracking WHERE ObjectiveId = @ObjectiveId);
	    
		IF(@contain != 0)
		BEGIN
		-- get Min month from GoalTraking conslulting 
		  SET @date = (SELECT MIN(DATEPART(MM,GoalTracking.FinishDate)) FROM GoalTracking where ObjectiveId = @ObjectiveId);

		 -- check which is the Quater
		 IF(@date >=1 AND @date <= 3)
			SET @Quater= 1	
		 ELSE IF(@date >=4 AND @date <= 6)	
			SET @Quater= 2
		 ELSE IF(@date >=7 AND @date <= 9)	
			SET @Quater= 3	
		 ELSE IF(@date >=10 AND @date <= 12)	
			SET @Quater= 4	
		
		-- UPDATE Objective with a new Quarter 
		UPDATE Objective SET Quarter = @Quater where ObjectiveId = @ObjectiveId
			
		END
		ELSE 
		BEGIN
			-- IF Objective doesn't have Goals, the Quarter is one
			UPDATE Objective SET Quarter = 1 where ObjectiveId = @ObjectiveId
		END
  END;
  
CLOSE RegistryCursor

DEALLOCATE RegistryCursor
