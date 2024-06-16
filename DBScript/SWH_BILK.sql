
183762
181873


--47.3774903
--19.1203315
--NOD:229031

--47.375785
--19.120828
--NOD:229031

--229031 => 
--update DEP_DEPOT set NOD_ID = 40741

select * into _EDG_EDGE_20171219 from EDG_EDGE
go


alter table EDG_EDGE add ORI_RZN_ZONECODE_20171219 TY_NAME null
go
alter table EDG_EDGE add ORI_MAXWEIGHT_20171219 TY_NAME null
go
alter table EDG_EDGE add ORI_MAXHEIGHT_20171219 TY_NAME null
go
alter table EDG_EDGE add ORI_MAXWIDTH_20171219 TY_NAME null
go
alter table EDG_EDGE add BILK_20171219 TY_BVALUE null
go
update EDG_EDGE set ORI_RZN_ZONECODE_20171219 = RZN_ZONECODE, BILK_20171219=0,
ORI_MAXWEIGHT_20171219 =  EDG_MAXWEIGHT, ORI_MAXHEIGHT_20171219 = EDG_MAXHEIGHT, ORI_MAXWIDTH_20171219 = EDG_MAXWIDTH
go

create PROCEDURE UpdateBilk
	@lat float,
	@lng float
AS
begin
declare @LatLngDivider float
set @LatLngDivider = 1000000
declare @area float
set @area = 3000/@LatLngDivider


--X --> lng, Y --> lat

update EDG
set RZN_ZONECODE='', EDG_MAXWEIGHT = 0, EDG_MAXHEIGHT = 0, EDG_MAXWIDTH = 0, BILK_20171219=1
from EDG_EDGE EDG
inner join NOD_NODE NOD1 on NOD1.ID = edg.NOD_NUM
inner join NOD_NODE NOD2 on NOD2.ID = edg.NOD_NUM2
where BILK_20171219 = 0 and NOD1.ZIP_NUM<'2000' and  NOD1.ZIP_NUM>'0'
and dbo.fnDistanceBetweenSegmentAndPoint( NOD1.NOD_YPOS/@LatLngDivider, NOD1.NOD_XPOS/@LatLngDivider,
    NOD2.NOD_YPOS/@LatLngDivider, NOD2.NOD_XPOS/@LatLngDivider,  @lat, @lng) < @area



end 
go 

exec UpdateBilk	47.377417 ,	19.120923
go

exec UpdateBilk	47.375785,	19.120828
go

exec UpdateBilk	47.376374,	19.119164
go

exec UpdateBilk	47.375082,	19.117443
go

exec UpdateBilk	47.374217,	19.118559
go

exec UpdateBilk	47.374594,	19.12128
go

exec UpdateBilk	47.37422	  , 19.121806
go

exec UpdateBilk	47.375419,	19.121706
go

exec UpdateBilk	47.375866,	19.123555
go

exec UpdateBilk	47.373918,	19.123665
go

exec UpdateBilk	47.373126,	19.123762
go

exec UpdateBilk	47.374866,	19.124704
go

exec UpdateBilk	47.3754952,	19.1255363
go

exec UpdateBilk	47.374001, 19.12502
go

exec UpdateBilk	47.372433,	19.124779
go

exec UpdateBilk	47.373237,	19.126767
go

exec UpdateBilk	47.373607,	19.114505
go

exec UpdateBilk	47.374215,	19.118571
go

exec UpdateBilk	47.373526,	19.119341
go

exec UpdateBilk	47.372233,	19.116734
go

exec UpdateBilk  	47.37653,	19.126704
go

--sajat
exec UpdateBilk  	47.3709195,	19.1181850
go
exec UpdateBilk  	47.3764996,	19.1290855
go
exec UpdateBilk  	47.3715590, 19.1145802
go
exec UpdateBilk  	47.3833576, 19.1231632
go
exec UpdateBilk  	47.3802775, 19.1275406
go
exec UpdateBilk  	47.37841765, 19.1301155
go
exec UpdateBilk  	47.3773424, 19.1313171
go
exec UpdateBilk  	47.3789698, 19.1299438
go
exec UpdateBilk  	47.3811202, 19.1275835
go
exec UpdateBilk  	47.3827184, 19.1254377
go
exec UpdateBilk  	47.3851592, 47.3851592
go
exec UpdateBilk  	47.3851592, 19.1211891
go
exec UpdateBilk  	47.3711521, 19.1191292
go


drop procedure UpdateBilk

truncate table DST_DISTANCE 
select * from EDG_EDGE where BILK_20171219 = 1
INSERT INTO [dbo].[EDG_EDGE]
           ([RDT_VALUE]
           ,[EDG_NUM]
           ,[EDG_LENGTH]
           ,[EDG_STRNUM1]
           ,[EDG_STRNUM2]
           ,[EDG_STRNUM3]
           ,[EDG_STRNUM4]
           ,[LASTDATE]
           ,[SELECTED]
           ,[EDG_ONEWAY]
           ,[RZN_ZONECODE]
           ,[EDG_DESTTRAFFIC]
           ,[EDG_ETLCODE]
		   , NOD_NUM
           ,NOD_NUM2
		  
           )
     VALUES
 --          (5,999999999,1,0,0,0,0, GETDATE(), 0, 0, '', 1, '', 229031, 218297)
          (5,999999999,1,0,0,0,0, GETDATE(), 0, 0, '', 1, '', 229030, 218298)


INSERT INTO [dbo].[EDG_EDGE]
           ([RDT_VALUE]
           ,[EDG_NUM]
           ,[EDG_LENGTH]
           ,[EDG_STRNUM1]
           ,[EDG_STRNUM2]
           ,[EDG_STRNUM3]
           ,[EDG_STRNUM4]
           ,[LASTDATE]
           ,[SELECTED]
           ,[EDG_ONEWAY]
           ,[RZN_ZONECODE]
           ,[EDG_DESTTRAFFIC]
           ,[EDG_ETLCODE]
		   , NOD_NUM
           ,NOD_NUM2
		  
           )
     VALUES
 --          (5,999999999,1,0,0,0,0, GETDATE(), 0, 0, '', 1, '', 229031, 218297)
          (5,999999999,1,0,0,0,0, GETDATE(), 0, 0, '', 1, '', 147546, 183760)







update EDG_EDGE set EDG_STRNUM4 = 1000 + EDG_STRNUM4 where BILK_20171219 = 1 and EDG_STRNUM4 < '1000'
--update EDG_EDGE set RDT_VALUE = RDT_VALUE - 1000 where BILK_20171219 = 1i



print 1235/60
print 1235-(1235/60)*60

/*
update EDG
select *
--set RZN_ZONECODE='', EDG_MAXWEIGHT = 0, EDG_MAXHEIGHT = 0, EDG_MAXWIDTH = 0, BILK_20171219=1
from EDG_EDGE EDG
inner join NOD_NODE NOD1 on NOD1.ID = edg.NOD_NUM
inner join NOD_NODE NOD2 on NOD2.ID = edg.NOD_NUM2
where BILK_20171219 = 0
and  upper( EDG.EDG_NAME_ENC) = 'EURÓPA UTCA%' and nod1.ZIP_NUM < 2000
*/
