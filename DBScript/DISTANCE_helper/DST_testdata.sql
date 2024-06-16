truncate table DST_DISTANCE

INSERT INTO [CT_DR3].[dbo].[DST_DISTANCE]
           ([NOD_ID_FROM]
           ,[NOD_ID_TO]
           ,[SPP_ID]
           ,[DST_DISTANCE]
           ,[DST_TIME]
           ,[DST_PATH]
           ,[DST_EDGES]
           ,[DST_POINTS]
           ,[RZN_ID_LIST])
     VALUES
           (13,15, 1, 1, 1, '', null, null
           ,'1,2,3,4,5,6,7,8,9,10,11,12,13,14,15')
GO

INSERT INTO [CT_DR3].[dbo].[DST_DISTANCE]
           ([NOD_ID_FROM]
           ,[NOD_ID_TO]
           ,[SPP_ID]
           ,[DST_DISTANCE]
           ,[DST_TIME]
           ,[DST_PATH]
           ,[DST_EDGES]
           ,[DST_POINTS]
           ,[RZN_ID_LIST])
     VALUES
           (13,15, 1, 1, 1, '', null, null
           ,'1,3,5,11')
GO

INSERT INTO [CT_DR3].[dbo].[DST_DISTANCE]
           ([NOD_ID_FROM]
           ,[NOD_ID_TO]
           ,[SPP_ID]
           ,[DST_DISTANCE]
           ,[DST_TIME]
           ,[DST_PATH]
           ,[DST_EDGES]
           ,[DST_POINTS]
           ,[RZN_ID_LIST])
     VALUES
           (13,15, 1, 1, 1, '', null, null
           ,'1,3,5,7,10,14')
GO

INSERT INTO [CT_DR3].[dbo].[DST_DISTANCE]
           ([NOD_ID_FROM]
           ,[NOD_ID_TO]
           ,[SPP_ID]
           ,[DST_DISTANCE]
           ,[DST_TIME]
           ,[DST_PATH]
           ,[DST_EDGES]
           ,[DST_POINTS]
           ,[RZN_ID_LIST])
     VALUES
           (13,15, 1, 1, 1, '', null, null
           ,'3,4,5,10,12,14')
GO

INSERT INTO [CT_DR3].[dbo].[DST_DISTANCE]
           ([NOD_ID_FROM]
           ,[NOD_ID_TO]
           ,[SPP_ID]
           ,[DST_DISTANCE]
           ,[DST_TIME]
           ,[DST_PATH]
           ,[DST_EDGES]
           ,[DST_POINTS]
           ,[RZN_ID_LIST])
     VALUES
           (13,15, 1, 1, 1, '', null, null
           ,'5,12,14,15')
GO
