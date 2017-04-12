CREATE view VW_MembersInputReport
AS
SELECT s.SurveyId, 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter,  q.Text,  sr.ResponseId, sr.ResourceId, Quarter as QuarterId, year 
FROM Survey s
INNER JOIN Question q
on q.SurveyId =  s.SurveyId
INNER JOIN SurveyResponse sr 
on sr.QuestionId = q.QuestionId
WHERE  q.QuestionType =1 
AND SurveyType = 1