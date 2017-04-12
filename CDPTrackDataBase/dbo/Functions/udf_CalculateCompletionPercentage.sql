CREATE FUNCTION [dbo].[udf_CalculateCompletionPercentage]
(
	@Quarter int,
	@Year int,
	@SurveyType int,
	@Location varchar(3) = null,
	@EmployeeType varchar(15) = null
)
RETURNS @CompletionPercentage TABLE
(
	LocationId int,
	Name varchar(20),
	Anwsered int,
	Total int,
	CompletionPercentage decimal(5,2)
)
AS
	BEGIN
		-- start the functiom to identify the survey completion
		-- var declarations
		DECLARE @calculatedSurveyId INT


		-- retrieving the SurveyId based on SurveyType, Year and Quarter
		SELECT @calculatedSurveyId = SurveyId
		FROM Survey 
		WHERE Quarter = @Quarter AND
			  Year = @Year AND
			  SurveyType = @SurveyType;

		-- calculate the total of employees per site
		WITH cteTotalResourcesPerSite (LocationId, Name, Total)
		AS
		(
			SELECT R.LocationId, L.Name, COUNT(R.LocationId) AS Total
			FROM Resource AS R
			INNER JOIN Employee AS Em ON R.ResourceId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Location AS L ON R.LocationId = L.ID 
			WHERE R.IsEnable = 1
			AND ( (@Location is null) OR (L.Abbreviation = @Location))
			GROUP BY R.LocationId, L.Name
		)

		-- populate the return table 
		INSERT INTO @CompletionPercentage	-- populating table to return from query
			SELECT Res.LocationId, L.Name, COUNT(Res.LocationId) AS Answered, CTE.Total, CAST((((COUNT(Res.LocationId) * 1.00)/ nullif(CTE.Total, 0))*100) AS DECIMAL(5,2)) AS CompletionPercentage
			FROM SurveyResource AS SR
			INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
			INNER JOIN [Resource] Res ON Res.ResourceId = Sres.ResourceEvaluatedId AND Res.IsEnable = 1
			INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Location AS L ON Res.LocationId = L.ID 
			INNER JOIN cteTotalResourcesPerSite as CTE ON Res.LocationId = CTE.LocationId
			WHERE SR.SurveyId = @calculatedSurveyId AND 
				  Res.IsEnable = 1
				  AND ( (@Location is null) OR (L.Abbreviation = @Location))
			GROUP BY Res.LocationId, L.Name, CTE.Total

		RETURN
	END

GO