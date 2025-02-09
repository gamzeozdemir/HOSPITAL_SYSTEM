USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[SearchDoctorsBySpecialty]    Script Date: 11.12.2024 14:29:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SearchDoctorsBySpecialty]
    @Specialty NVARCHAR(50) -- Uzmanl�k alan�n� parametre olarak al�r
AS
BEGIN
    SELECT 
        DocId AS DoctorID, 
        DocName AS DoctorName, 
        Specialty AS Expertise, 
        Phone, 
        Email, 
        Experience AS YearsOfExperience
    FROM 
        Doctor
    WHERE 
        Specialty = @Specialty;  -- Uzmanl�k alan�na g�re filtreleme
END;
GO


