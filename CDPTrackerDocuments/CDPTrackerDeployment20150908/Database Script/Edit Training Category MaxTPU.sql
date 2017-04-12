go
use CDPTrack;
go

go
UPDATE TrainingCategory SET TDU	= 1, MaxTDU = 9, 
Description = 'Write a detailed book recommendation for Tiempo Library or the Bulletin. 1 TDU per book recommendation. You may earn up to 9 points in this category per quarter.' WHERE TrainingCategory.TrainingCategoryId = 1;
go
UPDATE TrainingCategory SET TDU = 1, MaxTDU = 15,
Description	= 'Complete Soft skills training program, Webinar.  1 TDU per training. You may earn up to 15 points in this category per quarter.' WHERE TrainingCategory.TrainingCategoryId = 2;
go
UPDATE TrainingCategory SET TDU = 1, MaxTDU = 9,
Description	= 'Tiempo official technical trainings. Coursera and other online courses with exercises and evaluations. 1 TDU per Lecture only trainings 3 TDUs per interactive course. You may earn up to 9 points in this category per quarter.' WHERE TrainingCategory.TrainingCategoryId = 3;
go
UPDATE TrainingCategory SET TDU	=3, MaxTDU = 13,
Description = 'Technical trainings, not process related. Training will have to go through training policy training and TTT to make it official. 3 TDUs per Lecture only trainings 5 TDUs per interactive course.  You may earn up to 13 points in this category per quarter.' WHERE TrainingCategory.TrainingCategoryId = 4;
go
UPDATE TrainingCategory SET TDU	= 5, MaxTDU = 5,
Description	= 'Have constant meetings with coachee, exercises. 5 TDUs per coachee. You may earn up to 5 points in this category per quarter.' WHERE TrainingCategory.TrainingCategoryId = 5;
go
UPDATE TrainingCategory SET MaxTDU = 12,
Description = 'Each item in this category equals 0 points. You may earn up to 12 points in this category per quarter.' WHERE TrainingCategory.TrainingCategoryId = 6;
go

SELECT * FROM TrainingCategory;