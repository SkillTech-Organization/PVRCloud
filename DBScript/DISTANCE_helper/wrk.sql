
select * from (
--Meghatározzuk, milyen távolságrekordokra van szükésgünk  a tervben:
--    A tervben szereplõ lerakók között képezünk NOD_FROM_ID NOD_TO_ID távolságokat. 
--    Ezekhez hozzárakjuk a tervben lévõ jármûvek RESTZONE -it. 
select NOD_FROM_ID, NOD_TO_ID, RESTZONES  from
(
	select  NOD_FROM_ID, NOD_TO_ID, RESTZONES   from 
	(select NOD_FROM.ID as NOD_FROM_ID, NOD_TO.ID as NOD_TO_ID  
	 from (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS 
		   union 
		   select distinct NOD_ID as ID from DEP_DEPOT DEP 
		   inner join TOD_TOURORDER TOD on TOD.DEP_ID = DEP.ID and TOD.PLN_ID = 687
		   ) NOD_FROM 
	inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS  
	union 
	select distinct NOD_ID as ID from DEP_DEPOT DEP 
	 inner join TOD_TOURORDER TOD on TOD.DEP_ID = DEP.ID and TOD.PLN_ID = 687
	 ) NOD_TO on NOD_TO.ID <> NOD_FROM.ID 
	) ALLNODES
	left outer join 
	(select distinct
	  stuff( 
	  ( 
		  select ',' + convert( varchar(MAX), TRZX.RZN_ID ) 
		  from TRZ_TRUCKRESTRZONE TRZX 
		  where TRZX.TRK_ID = TPL.TRK_ID
		  order by TRZX.RZN_ID  
		  FOR XML PATH('') 
	  ), 1, 1, '') as RESTZONES 
	  from TPL_TRUCKPLAN TPL
	  where TPL.PLN_ID = 687 
	) ALLRSTZ on ALLRSTZ.RESTZONES is not null
	where  NOD_FROM_ID = 15
) PLANDIST
--kivonjuk a létezõ távolságokat
EXCEPT
select DST.NOD_ID_FROM as  NOD_FROM_ID, DST.NOD_ID_TO as NOD_TO_ID, DST.RST_ID_LIST as RESTZONES from DST_DISTANCE DST 
) NEEDDST
inner join NOD_NODE NOD_FROM on NOD_FROM.ID = NEEDDST.NOD_FROM_ID 
inner join NOD_NODE NOD_TO on NOD_TO.ID = NEEDDST.NOD_TO_ID 

select DST.NOD_ID_FROM as  NOD_FROM_ID, DST.NOD_ID_TO as NOD_TO_ID, DST.RST_ID_LIST as RESTZONES from DST_DISTANCE DST 
