declare @PLN_ID int
set @PLN_ID = 696

/* a megfelelõ PLN_ID megkeresése 
select ID, PLN_NAME, PLN_DATE_B, PLN_DATE_E from PLN_PUBLICATEDPLAN
order by ID desc
*/

--globál eredmény
--Túra sorszáma	Rendszám	Indulási idõ	Érkezési idõ	Megrendelések száma	Összes mennyiség I.	Összes mennyiség II.	Futott távolság	Költség
--1	AUCH1	6:45	13:10	8	55	1200	600	61000

;with CTE as (
select 
DENSE_RANK ( ) OVER ( order by  PTP_turastart.ID) as Túra_sorszáma,  
TPL.ID as TPL_ID, TPL.TRK_ID,
PTP_turastart.ID as PTP_turastart_ID, PTP_turastart.PTP_ORDER as PTP_turastart_ORDER,
PTP_turastart.PTP_DEPTIME as Indulási_idõ,
(select top 1 PTP_te.PTP_ORDER from PTP_PLANTOURPOINT PTP_te where PTP_te.TPL_ID=TPL.ID and PTP_te.PTP_TYPE = 1 and  PTP_te.PTP_ORDER > PTP_turastart.PTP_ORDER order by PTP_te.PTP_ORDER asc) as PTP_turaendOrder,
(select top 1 PTP_te.PTP_ARRTIME from PTP_PLANTOURPOINT PTP_te where PTP_te.TPL_ID=TPL.ID and PTP_te.PTP_TYPE = 1 and  PTP_te.PTP_ORDER > PTP_turastart.PTP_ORDER order by PTP_te.PTP_ORDER asc) as Érkezési_idõ,
(select count(*)  from PTP_PLANTOURPOINT PTP_cnt where PTP_cnt.TPL_ID=TPL.ID and PTP_cnt.PTP_TYPE = 2 and PTP_cnt.PTP_ORDER > PTP_turastart.PTP_ORDER and PTP_cnt.PTP_ORDER < PTP_turaend.PTP_ORDER) as Megrendelések_száma,
(select sum(TOD_QTY)  from PTP_PLANTOURPOINT PTP join TOD_TOURORDER TOD on TOD.ID = PTP.TOD_ID where PTP.TPL_ID=TPL.ID and PTP.PTP_TYPE = 2 and PTP.PTP_ORDER > PTP_turastart.PTP_ORDER and PTP.PTP_ORDER < PTP_turaend.PTP_ORDER) as Összes_mennyiség_I,
(select sum(TOD_VOLUME)  from PTP_PLANTOURPOINT PTP join TOD_TOURORDER TOD on TOD.ID = PTP.TOD_ID where PTP.TPL_ID=TPL.ID and PTP.PTP_TYPE = 2 and PTP.PTP_ORDER > PTP_turastart.PTP_ORDER and PTP.PTP_ORDER < PTP_turaend.PTP_ORDER) as Összes_mennyiség_II,
(select sum(PTP_DISTANCE)  from PTP_PLANTOURPOINT PTP_sum where PTP_sum.TPL_ID=TPL.ID and PTP_sum.PTP_ORDER >= PTP_turastart.PTP_ORDER and PTP_sum.PTP_ORDER <= PTP_turaend.PTP_ORDER) as Futott_távolság,
(select sum(PTP_TOLL)      from PTP_PLANTOURPOINT PTP_sum where PTP_sum.TPL_ID=TPL.ID and PTP_sum.PTP_ORDER >= PTP_turastart.PTP_ORDER and PTP_sum.PTP_ORDER <= PTP_turaend.PTP_ORDER) as PTP_TOLL
from PLN_PUBLICATEDPLAN PLN
inner join TPL_TRUCKPLAN TPL on TPL.PLN_ID = PLN.ID
inner join PTP_PLANTOURPOINT PTP_turastart on PTP_turastart.TPL_ID = TPL.ID and PTP_turastart.PTP_TYPE = 0
inner join .PTP_PLANTOURPOINT PTP_turaend ON  PTP_turaend.TPL_ID = TPL.ID AND PTP_turaend.PTP_ORDER = 
        (select min(PTP.PTP_ORDER) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID = TPL.ID AND PTP.PTP_ORDER > PTP_turastart.PTP_ORDER AND PTP.PTP_TYPE = 1)
where PLN.ID = @PLN_ID
) 
select TPL_ID, Túra_sorszáma,  TRK.TRK_REG_NUM as Rendszám,  Indulási_idõ, Érkezési_idõ, Megrendelések_száma, Összes_mennyiség_I, Összes_mennyiség_II, Futott_távolság, 
Futott_távolság/1000 * TFP.TFP_KMCOST + TFP.TFP_FIXCOST/ (select COUNT(*) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID=CTE.TPL_ID and PTP.PTP_TYPE = 0)  as Költség
from CTE
left outer join TRK_TRUCK TRK on TRK.ID = CTE.TRK_ID
left outer join TFP_TARIFFPROF TFP ON TRK.TFP_ID = TFP.ID 
order by  Túra_sorszáma


/*
Túra sorszáma	Rendszám	Túrapont sorszáma	Megrendelés azonosító	Érkezés	Távozás	Mennyiség 1.	Mennyiség II.	Távolság
1	AUCH1	1	0	5:15	6:15	0	0	0
1	AUCH1	2	123	6:58	7:15	8	110	13400

*/
select 
DENSE_RANK ( ) OVER ( order by  PTP_turastart.ID) as Túra_sorszáma,  
TRK.TRK_REG_NUM as Rendszám,
PTP.PTP_ORDER+1 as Túrapont_sorszáma,
ORD.ORD_NUM as Megrendelés_azonosító,
PTP.PTP_ARRTIME as Érkezés,
PTP.PTP_DEPTIME as Távozás,
TOD.TOD_QTY as  Mennyiség_1,
TOD.TOD_VOLUME as  Mennyiség_II,
PTP.PTP_DISTANCE as Távolság
from PLN_PUBLICATEDPLAN PLN
inner join TPL_TRUCKPLAN TPL on TPL.PLN_ID = PLN.ID 
inner join PTP_PLANTOURPOINT PTP_turastart on PTP_turastart.TPL_ID = TPL.ID and PTP_turastart.PTP_TYPE = 0
inner join .PTP_PLANTOURPOINT PTP_turaend ON  PTP_turaend.TPL_ID = TPL.ID AND PTP_turaend.PTP_ORDER = 
        (select min(PTP.PTP_ORDER) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID = TPL.ID AND PTP.PTP_ORDER > PTP_turastart.PTP_ORDER AND PTP.PTP_TYPE = 1)
inner join PTP_PLANTOURPOINT PTP on PTP.TPL_ID = TPL.ID and PTP.PTP_ORDER >= PTP_turastart.PTP_ORDER and PTP.PTP_ORDER <= PTP_turaend.PTP_ORDER 
inner join TRK_TRUCK TRK on TRK.ID = TPL.TRK_ID
left outer join TOD_TOURORDER TOD on TOD.ID = PTP.TOD_ID
left outer join ORD_ORDER ORD on ORD.ID = TOD.ORD_ID
where PLN.ID = @PLN_ID
order by Túra_sorszáma, Túrapont_sorszáma
