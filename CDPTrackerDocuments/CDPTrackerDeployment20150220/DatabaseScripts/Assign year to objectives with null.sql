USE [CDPTrack]
GO
update Objective set year = 2014 where year is NULL and Objective like '%Unassign Objectives%'