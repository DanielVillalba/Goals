CREATE FUNCTION udf_Calculate_team_agreement 
(	
	/* 
	@meorwe VALUES 
		true or 1 = the me section of the graph
		false or 9 = the we section of the graph
	*/
	@quarter int, 
	@Year int,
	@meorwe bit,
	@Location varchar(3) = null,
	@EmployeeType varchar(15) = null
)

RETURNS @TempData table
(
    [Text] nvarchar(max),
	[Strongly_Agree] decimal(5,2),
	[Agree] decimal(5,2),
	[Neutral] decimal(5,2),
	[Disagree] decimal(5,2), 
	[Strongly_Disagree] decimal(5,2)
) 
AS
BEGIN
DECLARE @sequence_id int
declare @TempSec table
(
[Sequence] int
)

IF @meorwe = 1 
	INSERT INTO @TempSec
	SELECT [Sequence] FROM Question WHERE [Sequence] in(9,4,3,2,1)
else
	INSERT INTO @TempSec
	SELECT [Sequence] FROM Question WHERE [Sequence] in(8,7,6,5)

DECLARE sequence_cursor CURSOR FOR   

SELECT Q.[Sequence]  FROM Question Q
	INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 1 
	WHERE Q.[Sequence] IN (SELECT [Sequence] from @TempSec)
	ORDER BY Q.[Sequence]  

OPEN sequence_cursor  

FETCH NEXT FROM sequence_cursor   
INTO @sequence_id

WHILE @@FETCH_STATUS = 0  
BEGIN  

INSERT INTO @TempData
SELECT *  FROM (
		SELECT b.[Text],Answer, CAST((((counts * 1.00)/ nullif([all], 0))*100) AS DECIMAL(5,2)) AS AnswerPercentage FROM (SELECT [Text],Answer, COUNT(1) as counts 
			FROM  SurveyResponse SR
			INNER JOIN Response R ON R.ResponseId = SR.ResponseID
			INNER JOIN Question Q ON Q.QuestionId = SR.QuestionID 
			INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
			INNER JOIN [Resource] Res ON Res.ResourceId = Sres.ResourceEvaluatedId AND Res.IsEnable = 1
			INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Location AS L ON Res.LocationId = L.ID  AND ( (@Location is null) OR (L.Abbreviation = @Location))
			INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND [Sequence] = @sequence_id AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 1
			GROUP BY [Text],  Answer, q.[Text]) b,
		(SELECT  SUM(counts) as [all] FROM (
			SELECT Answer, COUNT(1) as counts FROM  SurveyResponse SR
			INNER JOIN Response R ON R.ResponseId = SR.ResponseID
			INNER JOIN Question Q ON Q.QuestionId = SR.QuestionID 
			INNER JOIN [SurveyResource] as Sres ON Sres.SurveyResourceID = SR.SurveyResourceID
			INNER JOIN [Resource] Res ON Res.ResourceId = Sres.ResourceEvaluatedId AND Res.IsEnable = 1
			INNER JOIN Employee AS Em ON Sres.ResourceEvaluatedId = Em.ResourceId  AND  ((@EmployeeType is null) OR (Em.Type = @EmployeeType))
			INNER JOIN Location AS L ON Res.LocationId = L.ID  AND ( (@Location is null) OR (L.Abbreviation = @Location))
			INNER JOIN Survey S ON S.SurveyID = Q.SurveyID AND [Sequence] = @sequence_id AND S.[Quarter] = @quarter AND S.[Year] = @Year AND S.SurveyType = 1
			GROUP BY Answer) a) c )
		AS SourceTable
PIVOT
(AVG(AnswerPercentage) FOR Answer IN ([Strongly Agree],[Agree],[Neutral],[Disagree], [Strongly Disagree])) AS PIVOTTABLE


FETCH NEXT FROM sequence_cursor   
    INTO @sequence_id
END   
CLOSE sequence_cursor;  
DEALLOCATE sequence_cursor;  

RETURN
END
GO
