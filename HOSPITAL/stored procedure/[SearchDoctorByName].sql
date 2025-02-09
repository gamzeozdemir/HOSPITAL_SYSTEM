USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[SearchDoctorByName]    Script Date: 11.12.2024 14:29:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SearchDoctorByName]
    @DocName NVARCHAR(50)
AS
BEGIN
    SELECT * 
    FROM Doctor
    WHERE DocName LIKE '%' + @DocName + '%';
END;
GO


