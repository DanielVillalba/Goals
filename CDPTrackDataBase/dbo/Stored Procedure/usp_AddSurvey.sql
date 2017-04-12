CREATE PROCEDURE [dbo].[usp_AddSurvey]
	@OldQuarter int,
	@OldYear int,
	@NewQuarter int,
	@NewYear int,
	@SurveyType int,
	@EnabledDate datetime
AS
	-- ************************************************************************************************************
	DECLARE @SurveyId int, @newSurveyId int
	DECLARE @SurveyText varchar(25)

	IF(@SurveyType = 1)
		BEGIN
			SET @SurveyText = 'Team Members Input Q'
		END
	ELSE IF(@SurveyType = 2)
		BEGIN
			SET @SurveyText = 'Managers Check for Q'
		END

	SELECT @SurveyId =  SurveyId
	FROM Survey 
	WHERE Quarter = @OldQuarter	AND
		  Year = @OldYear AND
		  SurveyType = @SurveyType

	SELECT  @newSurveyId = Max(Surveyid) +1 
	FROM Survey

	BEGIN TRY
		BEGIN TRAN
			INSERT INTO Survey
			VALUES(@newSurveyId , 'Survey Q'+ CONVERT(char(1), @newquarter) + ' '  + CONVERT(char(4), @newYear) , @SurveyText + CONVERT(char(1), @newquarter) + ' '  + CONVERT(char(4), @newYear), @SurveyType, 99, @EnabledDate, @newquarter, @newYear)
 
 			DECLARE @maxQuestionId int
			SELECT @maxQuestionId = max(QuestionId) 
			FROM Question 

			INSERT INTO Question
			SELECT @maxQuestionId +  ROW_NUMBER() over (order by QuestionId desc), @newSurveyId , Text, Updated, Sequence, QuestionType , QuestionChild, Required, DisplayWhenValue, VisibleForEmployee 
			FROM question
			WHERE SurveyId = @SurveyId


			DECLARE @maxQuestionResponseId int
			SELECT @maxQuestionResponseId  = Max(QuestionResponseId) 
			FROM QuestionResponse

			INSERT INTO [QuestionResponse]
			SELECT  QuestionId, ResponseId , @maxQuestionResponseId  + ROW_NUMBER() OVER (ORDER BY QuestionId DESC)
			FROM Question 
			CROSS JOIN Response r
			WHERE SurveyId = @newSurveyId AND
				  ResponseId <= 5

		COMMIT
	END TRY
	BEGIN CATCH 
		ROLLBACK
	END CATCH
	-- ************************************************************************************************************





GO