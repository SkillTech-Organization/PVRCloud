select * from
		(
		select  * from  
			(select NOD_FROM.ID as NOD_ID_FROM, NOD_TO.ID as NOD_ID_TO   
			 from (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS  
				   union  
				   select distinct NOD_ID as ID from DEP_DEPOT DEP  
				   inner join TOD_TOURORDER TOD on TOD.PLN_ID = 696 and TOD.DEP_ID = DEP.ID
				   ) NOD_FROM  
			inner join (select distinct NOD_ID as ID from WHS_WAREHOUSE WHS   
				   union  
				  select distinct NOD_ID as ID from DEP_DEPOT DEP  
				  inner join TOD_TOURORDER TOD on TOD.PLN_ID = 696 and TOD.DEP_ID = DEP.ID 
				  ) NOD_TO on NOD_TO.ID <> NOD_FROM.ID  
			) Q1, 
			(select RZN.RZN_ID_LIST, TRK.SPP_ID
			  from TPL_TRUCKPLAN TPL 
			  inner join v_trk_RZN_ID_LIST RZN on RZN.TRK_ID  = TPL.TRK_ID
			  inner join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID
			  where TPL.PLN_ID = 696
			  ) Q2) NODES 
	inner join DST_DISTANCE DST on DST.RZN_ID_LIST =NODES.RZN_ID_LIST and DST.NOD_ID_FROM = NODES.NOD_ID_FROM and  DST.NOD_ID_TO = NODES.NOD_ID_TO
order by 1,2,3,4	  
