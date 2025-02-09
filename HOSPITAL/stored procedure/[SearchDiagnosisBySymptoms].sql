USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[SearchDiagnosisBySymptoms]    Script Date: 11.12.2024 14:29:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SearchDiagnosisBySymptoms]
    @Symptoms NVARCHAR(50) -- Semptom parametresi
AS
BEGIN
    SELECT *
    FROM Diagnosis -- Teþhis tablosu adý
    WHERE Symptoms LIKE '%' + @Symptoms + '%';
END;
GO


