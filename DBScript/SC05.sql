truncate table MPP_MAPPLANPAR

/****** Object:  View [dbo].[v_PLTOURQTY]    Script Date: 10/21/2010 20:51:46 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_PLTOURQTY]'))
DROP VIEW [dbo].[v_PLTOURQTY]
GO

/****** Object:  View [dbo].[v_PLTOURQTY]    Script Date: 10/21/2010 20:51:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_PLTOURQTY]
AS
SELECT     TPL.ID AS TPL_ID, PTPS.PTP_ORDER, SUM(TOD.TOD_QTY) AS PLQTY, MIN(ISNULL(ORD.ORD_LOCKDATE, 0)) AS LCK, 
                      PTPE.PTP_ORDER AS PTPE_ORDER, SUM(TOD.TOD_VOLUME) AS PLVOL, COUNT(*) AS PLCNT
FROM         dbo.TPL_TRUCKPLAN TPL INNER JOIN
                      dbo.PTP_PLANTOURPOINT PTPS ON PTPS.TPL_ID = TPL.ID AND PTPS.PTP_TYPE = 0 INNER JOIN
                      dbo.PTP_PLANTOURPOINT PTPE ON PTPE.TPL_ID = TPL.ID AND PTPE.PTP_ORDER =
                          (SELECT     MIN(PTP.PTP_ORDER)
                            FROM          PTP_PLANTOURPOINT PTP
                            WHERE      PTP.TPL_ID = TPL.ID AND PTP.PTP_ORDER > PTPS.PTP_ORDER AND PTP.PTP_TYPE = 1) INNER JOIN
                      dbo.PTP_PLANTOURPOINT PTPA ON PTPA.TPL_ID = TPL.ID AND PTPA.PTP_ORDER >= PTPS.PTP_ORDER AND 
                      PTPA.PTP_ORDER <= PTPE.PTP_ORDER LEFT OUTER JOIN
                      dbo.TOD_TOURORDER TOD ON PTPA.TOD_ID = TOD.ID LEFT OUTER JOIN
                      dbo.ORD_ORDER ORD ON TOD.ORD_ID = ORD.ID LEFT OUTER JOIN
                      dbo.OTP_ORDERTYPE OTP ON ORD.OTP_ID = OTP.ID
WHERE     (OTP.OTP_VALUE = 1) OR
                      (OTP.OTP_VALUE = 3)
GROUP BY TPL.ID, PTPS.PTP_ORDER, PTPE.PTP_ORDER

GO

if not exists(select syscolumns.id from syscolumns join sysobjects on  syscolumns.ID = sysobjects.ID  where  sysobjects.name = 'APP_APPVER' and  syscolumns.name = 'APP_SERIAL') begin
	ALTER TABLE APP_APPVER ADD APP_SERIAL nvarchar(MAX)        not null default '';
end
go

