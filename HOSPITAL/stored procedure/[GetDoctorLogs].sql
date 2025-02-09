USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[GetDoctorLogs]    Script Date: 11.12.2024 14:27:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDoctorLogs]
AS
BEGIN
    SELECT * FROM Doctor_Log
    ORDER BY DeletedAt DESC; -- En son silinen kayýtlarý göstermek için
END;
GO


