--Script for adding Progress column 
USE CDPTrack

ALTER TABLE Objective ADD Progress int

ALTER TABLE Objective ADD CONSTRAINT fk_Progress
FOREIGN KEY (Progress)
REFERENCES ProgressEnum(Id)