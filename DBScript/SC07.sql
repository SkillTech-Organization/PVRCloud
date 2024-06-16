select distinct * from (
select NOD.ID as NOD_ID, EDG.ID as EDG_ID, NOD.ZIP_NUM, ZIP_CITY, EDG_NAME, 
EDG_STRNUM1, EDG_STRNUM2, EDG_STRNUM3, EDG_STRNUM4
from NOD_NODE NOD
inner join EDG_EDGE EDG on EDG.NOD_NUM = NOD.ID
inner join ZIP_ZIPCODE ZIP on ZIP.ZIP_NUM = NOD.ZIP_NUM
UNION
select NOD.ID as NOD_ID, EDG.ID as EDG_ID, NOD.ZIP_NUM, ZIP_CITY, EDG_NAME,
EDG_STRNUM1, EDG_STRNUM2, EDG_STRNUM3, EDG_STRNUM4
from NOD_NODE NOD
inner join EDG_EDGE EDG on EDG.NOD_NUM2 = NOD.ID
inner join ZIP_ZIPCODE ZIP on ZIP.ZIP_NUM = NOD.ZIP_NUM
) cc
where ZIP_NUM = 6726 and UPPER( ZIP_CITY) = 'SZEGED' and 
UPPER( EDG_NAME) like '%VEDRES%'