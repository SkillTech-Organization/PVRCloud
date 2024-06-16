select * into ETL_ETOLL_2018 from ETL_ETOLL
/*
1 Autópálya
2 Autóút
3 Foútvonal
4 Mellékút
5 Egyéb alárendelt út
6 Város (utca)
7 Fel/lehajtók, rámpák
*/
update	EDG
set RDT_VALUE = 3
from ETL_ETOLL ETL
inner join EDG_EDGE EDG on EDG_ETLCODE = ETL.ETL_CODE
where EDG_ETLCODE in ('47u215k641m', '47u215k641m', '47u215k641m', '47u215k641m', '47u218k76m', '47u218k76m', '47u218k76m', '47u218k76m', '4u140k441m', '4u141k265m', '4u151k191m', '6u137k300m', '6u139k86m')


update	EDG
set RDT_VALUE = 1
from ETL_ETOLL ETL
inner join EDG_EDGE EDG on EDG_ETLCODE = ETL.ETL_CODE
where EDG_ETLCODE in ('M5u16k500m', 'M5u13k0m', 'M5u13k635m', 'M5u16k500m', 'M5u16k500m', 'M5u14k460m', 'M5u14k460m', 'M30u1k550m', 'M30u1k550m', 'M5u13k0m', 'M5u13k0m', 'M5u13k635m', 'M5u13k635m', 'M5u14k460m', 'M5u14k460m', 'M5u16k500m')


select RDT_VALUE, * from ETL_ETOLL ETL
inner join EDG_EDGE EDG on EDG.EDG_ETLCODE = ETL.ETL_CODE
where EDG.RDT_VALUE in (7) and ETL_J2_TOLL_KM > 48

update	ETL
set ETL_J2_TOLL_KM = 55.44, ETL_J3_TOLL_KM = 77.78, ETL_J4_TOLL_KM  = 120.4
from ETL_ETOLL ETL
inner join EDG_EDGE EDG on EDG.EDG_ETLCODE = ETL.ETL_CODE
where EDG.RDT_VALUE in (1,2)


update	ETL
set ETL_J2_TOLL_KM = 23.58, ETL_J3_TOLL_KM = 40.83, ETL_J4_TOLL_KM  = 75.10
from ETL_ETOLL ETL
inner join EDG_EDGE EDG on EDG.EDG_ETLCODE = ETL.ETL_CODE
where EDG.RDT_VALUE in (3,4,5,6)

update	ETL_ETOLL 
set ETL_J2_TOLL_FULL = ETL_J2_TOLL_KM*ETL_LEN_KM/1000, 
ETL_J3_TOLL_FULL = ETL_J3_TOLL_KM*ETL_LEN_KM/1000, 
ETL_J4_TOLL_FULL  = ETL_J4_TOLL_KM*ETL_LEN_KM/1000


