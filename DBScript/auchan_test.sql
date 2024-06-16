declare @PLN_ID int
set @PLN_ID = 696

/* a megfelel� PLN_ID megkeres�se 
select ID, PLN_NAME, PLN_DATE_B, PLN_DATE_E from PLN_PUBLICATEDPLAN
order by ID desc
*/

--glob�l eredm�ny
--T�ra sorsz�ma	Rendsz�m	Indul�si id�	�rkez�si id�	Megrendel�sek sz�ma	�sszes mennyis�g I.	�sszes mennyis�g II.	Futott t�vols�g	K�lts�g
--1	AUCH1	6:45	13:10	8	55	1200	600	61000

;with CTE as (
select 
DENSE_RANK ( ) OVER ( order by  PTP_turastart.ID) as T�ra_sorsz�ma,  
TPL.ID as TPL_ID, TPL.TRK_ID,
PTP_turastart.ID as PTP_turastart_ID, PTP_turastart.PTP_ORDER as PTP_turastart_ORDER,
PTP_turastart.PTP_DEPTIME as Indul�si_id�,
(select top 1 PTP_te.PTP_ORDER from PTP_PLANTOURPOINT PTP_te where PTP_te.TPL_ID=TPL.ID and PTP_te.PTP_TYPE = 1 and  PTP_te.PTP_ORDER > PTP_turastart.PTP_ORDER order by PTP_te.PTP_ORDER asc) as PTP_turaendOrder,
(select top 1 PTP_te.PTP_ARRTIME from PTP_PLANTOURPOINT PTP_te where PTP_te.TPL_ID=TPL.ID and PTP_te.PTP_TYPE = 1 and  PTP_te.PTP_ORDER > PTP_turastart.PTP_ORDER order by PTP_te.PTP_ORDER asc) as �rkez�si_id�,
(select count(*)  from PTP_PLANTOURPOINT PTP_cnt where PTP_cnt.TPL_ID=TPL.ID and PTP_cnt.PTP_TYPE = 2 and PTP_cnt.PTP_ORDER > PTP_turastart.PTP_ORDER and PTP_cnt.PTP_ORDER < PTP_turaend.PTP_ORDER) as Megrendel�sek_sz�ma,
(select sum(TOD_QTY)  from PTP_PLANTOURPOINT PTP join TOD_TOURORDER TOD on TOD.ID = PTP.TOD_ID where PTP.TPL_ID=TPL.ID and PTP.PTP_TYPE = 2 and PTP.PTP_ORDER > PTP_turastart.PTP_ORDER and PTP.PTP_ORDER < PTP_turaend.PTP_ORDER) as �sszes_mennyis�g_I,
(select sum(TOD_VOLUME)  from PTP_PLANTOURPOINT PTP join TOD_TOURORDER TOD on TOD.ID = PTP.TOD_ID where PTP.TPL_ID=TPL.ID and PTP.PTP_TYPE = 2 and PTP.PTP_ORDER > PTP_turastart.PTP_ORDER and PTP.PTP_ORDER < PTP_turaend.PTP_ORDER) as �sszes_mennyis�g_II,
(select sum(PTP_DISTANCE)  from PTP_PLANTOURPOINT PTP_sum where PTP_sum.TPL_ID=TPL.ID and PTP_sum.PTP_ORDER >= PTP_turastart.PTP_ORDER and PTP_sum.PTP_ORDER <= PTP_turaend.PTP_ORDER) as Futott_t�vols�g,
(select sum(PTP_TOLL)      from PTP_PLANTOURPOINT PTP_sum where PTP_sum.TPL_ID=TPL.ID and PTP_sum.PTP_ORDER >= PTP_turastart.PTP_ORDER and PTP_sum.PTP_ORDER <= PTP_turaend.PTP_ORDER) as PTP_TOLL
from PLN_PUBLICATEDPLAN PLN
inner join TPL_TRUCKPLAN TPL on TPL.PLN_ID = PLN.ID
inner join PTP_PLANTOURPOINT PTP_turastart on PTP_turastart.TPL_ID = TPL.ID and PTP_turastart.PTP_TYPE = 0
inner join .PTP_PLANTOURPOINT PTP_turaend ON  PTP_turaend.TPL_ID = TPL.ID AND PTP_turaend.PTP_ORDER = 
        (select min(PTP.PTP_ORDER) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID = TPL.ID AND PTP.PTP_ORDER > PTP_turastart.PTP_ORDER AND PTP.PTP_TYPE = 1)
where PLN.ID = @PLN_ID
) 
select TPL_ID, T�ra_sorsz�ma,  TRK.TRK_REG_NUM as Rendsz�m,  Indul�si_id�, �rkez�si_id�, Megrendel�sek_sz�ma, �sszes_mennyis�g_I, �sszes_mennyis�g_II, Futott_t�vols�g, 
Futott_t�vols�g/1000 * TFP.TFP_KMCOST + TFP.TFP_FIXCOST/ (select COUNT(*) from PTP_PLANTOURPOINT PTP where PTP.TPL_ID=CTE.TPL_ID and PTP.PTP_TYPE = 0)  as K�lts�g
from CTE
left outer join TRK_TRUCK TRK on TRK.ID = CTE.TRK_ID
left outer join TFP_TARIFFPROF TFP ON TRK.TFP_ID = TFP.ID 
order by  T�ra_sorsz�ma


/*
T�ra sorsz�ma	Rendsz�m	T�rapont sorsz�ma	Megrendel�s azonos�t�	�rkez�s	T�voz�s	Mennyis�g 1.	Mennyis�g II.	T�vols�g
1	AUCH1	1	0	5:15	6:15	0	0	0
1	AUCH1	2	123	6:58	7:15	8	110	13400

*/
select 
DENSE_RANK ( ) OVER ( order by  PTP_turastart.ID) as T�ra_sorsz�ma,  
TRK.TRK_REG_NUM as Rendsz�m,
PTP.PTP_ORDER+1 as T�rapont_sorsz�ma,
ORD.ORD_NUM as Megrendel�s_azonos�t�,
PTP.PTP_ARRTIME as �rkez�s,
PTP.PTP_DEPTIME as T�voz�s,
TOD.TOD_QTY as  Mennyis�g_1,
TOD.TOD_VOLUME as  Mennyis�g_II,
PTP.PTP_DISTANCE as T�vols�g
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
order by T�ra_sorsz�ma, T�rapont_sorsz�ma
