CREATE FUNCTION [dbo].[udf_Calculate_Promotion_Percentage]
(
	@Quarter int,
	@Year int,
	@Location varchar(3) = null,
	@EmployeeType varchar(15) = null
)
RETURNS @CompletionPercentage TABLE
(
	Answers varchar(50),
	CompletionPercentage decimal(5,2)
)
AS
	BEGIN
		DECLARE @PromReadyYes int, @PerformProbYes int, @AllOthers int, @AnswersTotal int


		SELECT @AnswersTotal = COUNT(Res.LocationId)
			FROM SurveyResource AS SR
			INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
			INNER JOIN [Resource] Res ON Res.ResourceId = Sres.ResourceEvaluatedId AND Res.IsEnable = 1
			INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Survey AS Sur ON Sur.Quarter = @Quarter AND Sur.Year = @Year AND Sur.SurveyType = 2 AND SR.SurveyId = Sur.SurveyId
			INNER JOIN Location AS L ON Res.LocationId = L.ID AND ((@Location is null) OR (L.Abbreviation = @Location))
	

		SELECT @PromReadyYes = COUNT(1) FROM  SurveyResponse SR
			INNER JOIN Response R ON R.ResponseId = SR.ResponseID AND R.Answer = 'Yes'
			INNER JOIN Question Q ON Q.QuestionId = SR.QuestionID 
			INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
			INNER JOIN Resource AS Res ON Sres.ResourceEvaluatedId = Res.ResourceId AND  Res.IsEnable = 1
			INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Location AS L ON Res.LocationId = L.ID  AND ((@Location is null) OR (L.Abbreviation = @Location))
			INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND [Sequence] = 4 AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 2
		GROUP BY Answer

		SELECT @PerformProbYes = COUNT(1) FROM  SurveyResponse SR
			INNER JOIN Response R ON R.ResponseId = SR.ResponseID AND R.Answer = 'Yes'
			INNER JOIN Question Q ON Q.QuestionId = SR.QuestionID 
			INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
			INNER JOIN Resource AS Res ON Sres.ResourceEvaluatedId = Res.ResourceId AND  Res.IsEnable = 1
			INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Location AS L ON Res.LocationId = L.ID  AND ((@Location is null) OR (L.Abbreviation = @Location))
			INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND [Sequence] = 6 AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 2
		GROUP BY Answer

		SET @AllOthers = (coalesce(@AnswersTotal, 0) - coalesce(@PerformProbYes, 0) - coalesce(@PromReadyYes, 0))

		IF (@AllOthers < 0)
			SET @AllOthers = 0

		IF (@AnswersTotal > 0)
		BEGIN
			INSERT INTO @CompletionPercentage
			SELECT 'Promotion ready', CAST((((coalesce(@PromReadyYes, 0) * 1.00)/ nullif(coalesce(@AnswersTotal, 0),0))*100) AS DECIMAL(5,2)) AS Percentage
			INSERT INTO @CompletionPercentage
			SELECT 'Performance problem', CAST((((coalesce(@PerformProbYes, 0) * 1.00)/ nullif(coalesce(@AnswersTotal,0), 0))*100) AS DECIMAL(5,2)) AS Percentage
			INSERT INTO @CompletionPercentage
			SELECT 'Not promotion ready, nor performance problem', CAST((((coalesce(@AllOthers, 0) * 1.00)/ nullif(coalesce(@AnswersTotal,0), 0))*100) AS DECIMAL(5,2)) AS Percentage
		END

		RETURN
	END

