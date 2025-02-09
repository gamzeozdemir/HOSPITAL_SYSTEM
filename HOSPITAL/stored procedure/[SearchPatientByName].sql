USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[SearchPatientByName]    Script Date: 11.12.2024 14:30:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SearchPatientByName]
    @PName VARCHAR(50)
AS
BEGIN
    SELECT * 
    FROM Patient
    WHERE PName LIKE '%' + @PName + '%'; -- Adýn kýsmi eþleþmesi
END;

GO


