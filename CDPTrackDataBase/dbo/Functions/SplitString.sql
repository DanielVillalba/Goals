create function SplitString
(
  @a varchar(max), 
  @delimiter varchar(20)
)
RETURNS @t TABLE(substr varchar(200))
as
begin
set @a = @a + @delimiter
;with a as
(
  select cast(1 as bigint) f1, charindex(@delimiter, @a) f2
  where len(@a) > 0
  union all
  select f2 + (len(@delimiter)) + 1, charindex(@delimiter, @a, f2+1)
  from a
  where f2 > 0
)
insert @t
select substring(@a, f1, f2 - f1) from a
where f1 < f2
return
end