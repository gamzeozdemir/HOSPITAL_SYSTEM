USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[GetPatientLogs]    Script Date: 11.12.2024 14:28:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPatientLogs]
AS
BEGIN
    SELECT * FROM Patient_Log
    ORDER BY DeletedAt DESC; -- En son silinen kayýtlarý göstermek için
END;
GO


