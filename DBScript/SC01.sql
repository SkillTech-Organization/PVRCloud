/****** Object:  View [dbo].[v_TPLANVOL]    Script Date: 07/20/2010 17:29:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[v_TPLANVOL]
AS
SELECT     dbo.TPL_TRUCKPLAN.ID AS TPL_ID, SUM(dbo.TOD_TOURORDER.TOD_VOLUME) AS TPLANVOL
FROM         dbo.TPL_TRUCKPLAN INNER JOIN
                      dbo.PTP_PLANTOURPOINT ON dbo.TPL_TRUCKPLAN.ID = dbo.PTP_PLANTOURPOINT.TPL_ID INNER JOIN
                      dbo.TOD_TOURORDER ON dbo.PTP_PLANTOURPOINT.TOD_ID = dbo.TOD_TOURORDER.ID INNER JOIN
                      dbo.ORD_ORDER ON dbo.TOD_TOURORDER.ORD_ID = dbo.ORD_ORDER.ID INNER JOIN
                      dbo.OTP_ORDERTYPE ON dbo.ORD_ORDER.OTP_ID = dbo.OTP_ORDERTYPE.ID
WHERE     (dbo.OTP_ORDERTYPE.OTP_VALUE = 1) OR
                      (dbo.OTP_ORDERTYPE.OTP_VALUE = 3)
GROUP BY dbo.TPL_TRUCKPLAN.ID

GO
