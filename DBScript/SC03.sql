IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MPP_MAPPLANPAR]') AND type in (N'U'))
DROP TABLE [dbo].[MPP_MAPPLANPAR];

create table MPP_MAPPLANPAR (
   ID                   TY_ID                IDENTITY(1,1) not null,
   USR_ID               TY_ID                not null,
   PLN_ID               TY_ID                not null,
   MPP_WINDOW           nvarchar(MAX)        not null default '',
   MPP_DOCK             nvarchar(MAX)        not null default '',
   MPP_PARAM             nvarchar(MAX)       not null default '',
   MPP_TGRID             nvarchar(MAX)       not null default '',
   MPP_PGRID             nvarchar(MAX)       not null default '',
   MPP_UGRID             nvarchar(MAX)       not null default '',
   constraint PK_MPP_MAPPLANPAR primary key  (ID)
);