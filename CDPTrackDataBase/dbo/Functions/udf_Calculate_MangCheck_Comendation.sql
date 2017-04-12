CREATE FUNCTION udf_Calculate_MangCheck_Comendation 
(	
	/* 
	SEQUENCE VALUES 
		8. How would you categorize this person according to the value he/she brings to your team?
		6 This person has a performance problem that I need to address immediately (and/or that is already being addressed)
		2 If I had to/needed to, I would choose this person again today to work within my team.
		1 I always go to this person when I need great results
	*/
	@quarter int, 
	@Year int,
	@Sequence int,
	@Location varchar(3) = null,
	@EmployeeType varchar(15) = null
)
RETURNS TABLE 
AS
RETURN 
(

SELECT CASE WHEN @Sequence = 8 THEN substring(Answer,0,4) ELSE  Answer END as Answers, CAST((((counts * 1.00)/ nullif([all], 0))*100) AS DECIMAL(5,2)) AS CompletionPercentage FROM (SELECT Answer, COUNT(1) as counts FROM  SurveyResponse SR
	INNER JOIN Response R ON R.ResponseId = SR.ResponseID
	INNER JOIN Question Q ON Q.QuestionId = SR.QuestionID 
	INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
	INNER JOIN [Resource] Res ON Res.ResourceId = Sres.ResourceEvaluatedId AND Res.IsEnable = 1
	INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
	INNER JOIN Location AS L ON Res.LocationId = L.ID  AND((@Location is null) OR (L.Abbreviation = @Location))
	INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND [Sequence] = @Sequence AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 2
	GROUP BY Answer) b,
(SELECT  SUM(counts) as [all] FROM (
	SELECT Answer, COUNT(1) as counts FROM  SurveyResponse SR
	INNER JOIN Response R ON R.ResponseId = SR.ResponseID
	INNER JOIN Question Q ON Q.QuestionId = SR.QuestionID 
	INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
	INNER JOIN [Resource] Res ON Res.ResourceId = Sres.ResourceEvaluatedId AND Res.IsEnable = 1
	INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
	INNER JOIN Location AS L ON Res.LocationId = L.ID  AND ( (@Location is null) OR (L.Abbreviation = @Location))
	INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND [Sequence] = @Sequence AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 2
	GROUP BY Answer) a) c

)
GO