USE [VThastane]
GO

/****** Object:  StoredProcedure [dbo].[SearchDoctorsBySpecialty]    Script Date: 11.12.2024 14:29:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SearchDoctorsBySpecialty]
    @Specialty NVARCHAR(50) -- Uzmanlýk alanýný parametre olarak alýr
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
        Specialty = @Specialty;  -- Uzmanlýk alanýna göre filtreleme
END;
GO


