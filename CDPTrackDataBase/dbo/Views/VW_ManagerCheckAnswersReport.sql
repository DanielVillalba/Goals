 

CREATE VIEW VW_ManagerCheckAnswersReport
AS 
 
SELECT 'Q' + STR(s.Quarter, 1) + STR(s.Year, 5) as Quarter, Quarter as QuarterId, Year, q.Text ,
		sr.QuestionId, sr.ResourceId, ISNULL(sr.ResponseId, 0 ) as ResponseId,  
		 sr.ResponseText, 
		 sres.ResourceEvaluatedId 
FROM Survey s  
inner join  Question q
on q.SurveyId = s.SurveyId
inner join SurveyResource sres 
on sres.SurveyId = s.SurveyId
inner join SurveyResponse sr 
on sr.SurveyResourceId = sres.SurveyResourceId
and sr.QuestionId = q.QuestionId
where QuestionType = 1
and s.SurveyType = 2