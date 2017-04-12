use [CDPTrack]
GO
ALTER TABLE Location ADD abbreviation VARCHAR(3) NULL;
GO
update Location set abbreviation = 'HMO' where Name = 'Hermosillo';
update Location set abbreviation = 'PHX' where Name = 'Phoenix';
update Location set abbreviation = 'MTY' where Name = 'Monterrey';
update Location set abbreviation = 'GDL' where Name = 'Guadalajara';