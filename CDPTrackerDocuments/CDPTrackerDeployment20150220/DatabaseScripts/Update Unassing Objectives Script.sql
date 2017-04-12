-- Script for Updating all Objectives when objective is equals to Unassign Objectives and year is equal to 2014
-- This line only Must be Executed one time
UPDATE dbo.Objective SET Quarter=1 where objective  like '%Unassign Objectives%'  and Year = 2014