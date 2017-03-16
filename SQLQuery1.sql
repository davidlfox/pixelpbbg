select * from AspNetUsers where Email='new@new.com'
update AspNetUsers set EmailConfirmed = 1 where Email='new@new.com' 
select * from territories
delete from territories where Name like '%testing%'
drop database [aspnet-PixelApp-20170207052120]