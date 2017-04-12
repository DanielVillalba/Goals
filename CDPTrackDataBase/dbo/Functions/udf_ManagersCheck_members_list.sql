CREATE FUNCTION [dbo].[udf_ManagersCheck_members_list] (
-- Parameters
@managerActiveDiretoryId int,
@managerDataBaseId int,
@startDateQuarter date,
@endDateQuarter date,

-- LastQuarter
@startLastDateQuarter date,
@endLastDateQuarter date
)

RETURNS @manager_members_list TABLE 
( 
  employeManagerId int,
  employeName varchar(150),
  employeId int,
  employeEvaluated bit,
  employeGoals bit,
  employeTeamMembersImput bit
)
 AS
 BEGIN 
     DECLARE
	 -- Table information
	    @employeManagerId int,
		@employeName varchar(150),
		@employeId int,
		@employeEvaluated bit,
		@employeGoals bit,
		@employeTeamMembersImput bit,

		-- Help Variables
		@numberEmployes int = (SELECT COUNT(*) FROM Employee e INNER JOIN [Resource] r ON r.ResourceId = e.ResourceId AND r.IsEnable = 1 WHERE e.ManagerId = @managerActiveDiretoryId), 
		@i int = 1;
       

	    WHILE (@i <= @numberEmployes)
		BEGIN
				SELECT 
						@employeManagerId = @managerActiveDiretoryId,
						@employeId = x.ResourceId, 
						@employeName = x.Name  
						FROM ( 
								SELECT ROW_NUMBER() 
								OVER (ORDER BY Name ASC) AS Row, 
								r.Name, DomainName, r.ResourceId  
								FROM Employee AS e 
								INNER JOIN Resource AS r 
								ON e.ResourceId =  r.ResourceId  
								WHERE ManagerId = @managerActiveDiretoryId 
								AND r.IsEnable = 1
								) 
						AS x
						WHERE x.Row = @i

				-- Employe have any goals?
				SELECT @employeGoals  = 
					CASE WHEN EXISTS ( 
					
					SELECT * 
					FROM GoalTracking 
					WHERE GoalTracking.ResourceId = @employeId 
					AND FinishDate  BETWEEN DATEADD(MONTH,3,@startDateQuarter) AND DATEADD(MONTH,3,@endDateQuarter)
					)				 
							  THEN 1
					          ELSE 0
					 END;
		  
		      -- Employe was evaluated?
				SELECT @employeEvaluated = 
						CASE WHEN  EXISTS ( SELECT * 
							   FROM SurveyResource AS sr 
								WHERE sr.SurveyType = 2
								AND  sr.ResourceEvaluatedId = @employeId
								AND sr.ResourceId =  @managerDataBaseId
							    AND sr.DateAnswered BETWEEN @startDateQuarter AND @endDateQuarter
								)
				       THEN 1
				       ELSE 0
						END;
	  		 
			 -- Employee send his Teamd Members Input form?
				SELECT @employeTeamMembersImput =
	                   CASE WHEN EXISTS( SELECT * 
										 FROM SurveyResource 
										 WHERE SurveyType = 1 
										 AND ResourceId = @employeId
										 AND SurveyResource.DateAnswered BETWEEN @startDateQuarter AND @endDateQuarter
										 )
						THEN 1
					     ELSE 0
					END;      

  
				INSERT @manager_members_list SELECT @employeManagerId, @employeName, @employeId, @employeEvaluated, @employeGoals, @employeTeamMembersImput;  

			SELECT	@i = @i + 1;
		END 
  RETURN
 END