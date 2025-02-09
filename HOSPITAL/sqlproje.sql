CREATE DATABASE VThastane;
USE VThastane;


SELECT*FROM LoginPage;

SELECT*FROM Adminn;

drop table LoginPage
--burayý unutma kullanmadýk

CREATE TRIGGER trg_User_Insert
ON LoginPage
AFTER INSERT
AS
BEGIN
    DECLARE @Username VARCHAR(25);

    -- Yeni eklenen kullanýcýnýn adýný al
    SELECT @Username = Username FROM INSERTED;

    PRINT 'Admin baþarýyla oluþturuldu ' + @Username + '!';
END;



RESTORE DATABASE VThastane
FROM DISK = 'C:\Yedekler\VThastane_backup_20241205_013513.bak'
WITH REPLACE;

ALTER DATABASE VThastane SET SINGLE_USER WITH ROLLBACK IMMEDIATE;



DELETE FROM [dbo].[LoginPage];
DELETE FROM [dbo].[Adminn];



CREATE TABLE Adminn (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
	[FirstName] VARCHAR(50) NOT NULL, -- Kullanýcý Adý
    [LastName] VARCHAR(50) NOT NULL, -- Kullanýcý Soyadý
    [Psword] NVARCHAR(6) NOT NULL,
    SuperAdmin BIT NOT NULL DEFAULT 0 -- 1: SuperAdmin, 0: Normal Admin
);
select*from Adminn;

DBCC CHECKIDENT ('[dbo].[Adminn]', RESEED, 1);
DBCC CHECKIDENT ('[dbo].[LoginPage]', RESEED, 1);

drop table Adminn
drop table [LoginPage]

CREATE TRIGGER trg_Adminn_Insert
ON Adminn
AFTER INSERT
AS
BEGIN
    -- LoginPage tablosuna kayýt ekleme
    INSERT INTO LoginPage (Username, Psword, Duty, aID)
    SELECT 
        CONCAT(FirstName, LastName) AS Username, -- Kullanýcý adý olarak Ad ve Soyad birleþtirilir
        Psword,                                -- Þifre ayný kalýr
        'Adminn',                              -- Görev olarak 'Adminn' atanýr
        AdminID                                -- Admin tablosunun ID'si eklenir
    FROM INSERTED;                             -- INSERTED sanal tablosundan yeni eklenen deðerler alýnýr
END;


  


CREATE TABLE [dbo].[LoginPage] (
    [LoginId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Username] VARCHAR(25) NOT NULL,
    [Psword] CHAR(6) NOT NULL,
    [Duty] VARCHAR(50) CHECK (Duty IN ('Adminn')) NOT NULL,
	aID INT ,
	FOREIGN KEY (aID) REFERENCES Adminn(AdminID) ON DELETE CASCADE
);

INSERT INTO [dbo].[Adminn] (FirstName, LastName, Psword, SuperAdmin)
VALUES 
('Ali', 'Kaya', '123456', 1),     -- Ali Kaya, SuperAdmin 
('Ayþe', 'Demir', 'abcdef', 0),   -- Ayþe Demir, Normal Admin
('Can', 'Korkmaz', '654321', 0),  -- Can Korkmaz, Normal Admin
('Hülya', 'Uzun', 'admin1', 0), -- Hülya Uzun, Normal Admin
('Murat', 'Polat', 'secure', 0); -- Murat Polat, Normal Admin

INSERT INTO [dbo].[LoginPage] (Username, Psword, Duty, aID)
VALUES 
('AliKaya', '123456', 'Adminn', 1),  -- Ali Kaya, SuperAdmin
('AyseDemir', 'abcdef', 'Adminn', 2),  -- Ayþe Demir, Normal Admin
('CanKorkmaz', '654321', 'Adminn', 3), -- Can Korkmaz, Normal Admin
('HulyaUzun', 'admin1', 'Adminn', 4), -- Hülya Uzun, Normal Admin
('MuratPolat', 'secure', 'Adminn', 5); -- Murat Polat, Normal Admin

create table [dbo].[Doctor](
[DocId] INT NOT NULL IDENTITY(1000,1) PRIMARY KEY,
[DocName] VARCHAR(50) NOT NULL,
[DocGen] VARCHAR(6) NOT NULL,
[Experience] VARCHAR(2) NOT NULL,
[License] VARCHAR(5) NOT NULL,
 Specialty NVARCHAR(50) NOT NULL, -- Branþ
  Phone CHAR(11),             -- Telefon
    Email NVARCHAR(50) CONSTRAINT CK_Email_Gmail CHECK (Email LIKE '%@gmail.com'),
    NationalID CHAR(11) NOT NULL UNIQUE -- TCKimlikNo
);

select*from Doctor_Log


CREATE TABLE [dbo].[Patient] (
    [PId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- Birincil anahtar
    [PName] VARCHAR(50) NOT NULL, -- Hasta adý
    [PAddress] VARCHAR(50) NOT NULL, -- Adres
    [PBirthDate] date NOT NULL, -- dg
    [PPhone] CHAR(11) NOT NULL, -- Telefon
    [PGen] VARCHAR(6) NOT NULL, -- Cinsiyet
    [BloodGroup] VARCHAR(3) NOT NULL, -- Kan grubu
    [MajorDisease] VARCHAR(50) NOT NULL, -- Hastalýk
    Email NVARCHAR(50) CONSTRAINT CK_Email2_Gmail CHECK (Email LIKE '%@gmail.com'),
    NationalID CHAR(11) NOT NULL UNIQUE ,-- TCKimlikNo
	AssignedDoctorID INT NULL,      -- Atanmýþ Doktor (Opsiyonel)
    FOREIGN KEY (AssignedDoctorID) REFERENCES Doctor(DocId) ON DELETE SET NULL
);

select*from Patient

CREATE TABLE Diagnosis (
    DId INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,  -- Hasta ID
	DoctorID INT NOT NULL,
    PatientName varchar(50) NULL,     
    Symptoms VARCHAR(50) NOT NULL, -- Taný Açýklamasý
    DiagnosticTest Varchar(50),
	Medicines varchar(50),
    FOREIGN KEY (PatientID) REFERENCES Patient(PId) ON DELETE CASCADE,

    FOREIGN KEY (DoctorID) REFERENCES Doctor(DocId) ON DELETE CASCADE

);



CREATE TABLE AdminTable_Log(
	UserID INT IDENTITY(1,1) PRIMARY KEY,
	[FirstName] VARCHAR(50) NOT NULL, -- Kullanýcý Adý
    [LastName] VARCHAR(50) NOT NULL, -- Kullanýcý Soyadý
    [Psword] NVARCHAR(50) NOT NULL,
	SuperAdmin BIT NOT NULL DEFAULT 0
	);


CREATE TRIGGER trg_AdminTable_Delete
ON Adminn
INSTEAD OF DELETE
AS
BEGIN
    -- Silinen kayýtlarý AdminTable_Log tablosuna ekle
    INSERT INTO AdminTable_Log (UserId, FirstName, LastName, Psword, SuperAdmin)
    SELECT AdminID, FirstName, LastName, Psword, SuperAdmin
    FROM DELETED;

    -- Adminn tablosundan silme iþlemini gerçekleþtirme
    DELETE FROM Adminn
    WHERE AdminID IN (SELECT AdminID FROM DELETED);

    -- Kayýtlarýn AdminTable_Log tablosuna taþýndýðýný belirten mesaj
    PRINT 'Record moved to AdminTable_Log and deleted from Adminn';
END;



select*from Adminn

select*from [LoginPage]

CREATE TABLE Doctor_Log (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    [DocId] INT NOT NULL ,
[DocName] VARCHAR(50) NOT NULL,
[DocGen] VARCHAR(6) NOT NULL,
[Experience] VARCHAR(2) NOT NULL,
[License] VARCHAR(5) NOT NULL,
 Specialty NVARCHAR(50) NOT NULL, -- Branþ
  Phone CHAR(11),             -- Telefon
    Email NVARCHAR(50),
    NationalID CHAR(11) NOT NULL , -- TCKimlikNo
    DeletedAt DATETIME NOT NULL DEFAULT GETDATE()
);

drop table Doctor_Log



-- Log tablosu
CREATE TABLE Patient_Log (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    PId INT NOT NULL,
    PName VARCHAR(50),
    PAddress VARCHAR(50),
    PBirthDate Date,
    PPhone VARCHAR(11),
    PGen VARCHAR(6),
    BloodGroup VARCHAR(3),
    MajorDisease VARCHAR(50),
    DeletedAt DATETIME NOT NULL DEFAULT GETDATE()
);
 select*from Patient_Log;
-- Log tablosu
CREATE TABLE Diagnosis_Log (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    DId INT NOT NULL,
    PatientId INT,
    PatientName VARCHAR(50),
    Symptoms VARCHAR(50),
    DiagnosticTest VARCHAR(50),
    Medicines VARCHAR(50),
    DeletedAt DATETIME NOT NULL DEFAULT GETDATE()
);



-- Log tablosu
CREATE TABLE LoginPage_Log (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    LoginId INT NOT NULL,
    Username VARCHAR(25),
    Psword CHAR(6),  -- Burada doðru sütun adýný kullanýyoruz
    DeletedAt DATETIME NOT NULL DEFAULT GETDATE()
);





create TRIGGER trg_Doctor_Delete
ON Doctor
INSTEAD OF DELETE
AS
BEGIN
    -- Silinen kaydý Doctor_Log tablosuna ekle
    INSERT INTO Doctor_Log (DocId, DocName, DocGen, Experience, License,Specialty,Phone,Email,NationalID, DeletedAt)
    SELECT DocId, DocName, DocGen, Experience, License,Specialty,Phone,Email,NationalID, GETDATE()
    FROM DELETED;

    -- Asýl Doctor tablosundan kaydý sil
    DELETE FROM Doctor
    WHERE DocId IN (SELECT DocId FROM DELETED);

    PRINT 'Record moved to Doctor_Log and deleted from Doctor';
END;

drop trigger trg_Doctor_Delete

DBCC CHECKIDENT ('Doctor', RESEED, 1000);




CREATE TRIGGER trg_Diagnosis_AfterDelete
ON Diagnosis
AFTER DELETE
AS
BEGIN
    -- Silinen kaydýn bilgilerini Diagnosis_Log tablosuna ekliyoruz
    INSERT INTO Diagnosis_Log (DId, PatientID, PatientName, Symptoms, DiagnosticTest, Medicines)
    SELECT DId, PatientID, PatientName, Symptoms, DiagnosticTest, Medicines FROM DELETED;

    -- Silinen kaydý Diagnosis tablosundan siliyoruz
    DELETE FROM Diagnosis WHERE DId IN (SELECT DId FROM DELETED);

    PRINT 'Record moved to Diagnosis_Log and deleted from Diagnosis table';
END;




CREATE TRIGGER trg_Patient_Delete 
ON Patient 
INSTEAD OF DELETE 
AS 
BEGIN
    -- Silinen kaydý Patient_Log tablosuna ekle
    INSERT INTO Patient_Log (PId, PName, PAddress, PBirthDate, PPhone, PGen, BloodGroup, MajorDisease)
    SELECT PId, PName, PAddress, PBirthDate, PPhone, PGen, BloodGroup, MajorDisease 
    FROM DELETED;
    
    -- Asýl Patient tablosundan kaydý sil
    DELETE FROM Patient
    WHERE PId IN (SELECT PId FROM DELETED);
    
    PRINT 'Record moved to Patient_Log and deleted from Patient';
END;



INSERT INTO Adminn (FirstName, LastName, Psword, SuperAdmin)
VALUES 
('Ali', 'Kaya', '123456', 1),
('Ayþe', 'Demir', 'abcdef', 0),
('Can', 'Korkmaz', '654321', 0),
('Hülya', 'Uzun', 'admin1', 0),
('Murat', 'Polat', 'secure', 0);

INSERT INTO LoginPage (Username, Psword, Duty, aID)
VALUES 
('admin1', '123456', 'Adminn', 1),
('admin2', 'abcdef', 'Adminn', 2),
('admin3', 'qwerty', 'Adminn', 3),
('admin4', 'zxcvbn', 'Adminn', 4),
('admin5', '111111', 'Adminn', 5);


INSERT INTO Doctor (DocName, DocGen, Experience, License, Specialty, Phone, Email, NationalID)
VALUES 
('Dr. Ali', 'Male', '15', '12345', 'Cardiology', '05001111111', 'ali@gmail.com', '11111111111'),
('Dr. Ayþe', 'Female', '10', '23456', 'Dermatology', '05002222222', 'ayse@gmail.com', '22222222222'),
('Dr. Fatma', 'Female', '12', '34567', 'Neurology', '05003333333', 'fatma@gmail.com', '33333333333'),
('Dr. Mehmet', 'Male', '20', '45678', 'Pediatrics', '05004444444', 'mehmet@gmail.com', '44444444444'),
('Dr. Ahmet', 'Male', '8', '56789', 'Orthopedics', '05005555555', 'ahmet@gmail.com', '55555555555'),
('Dr. Elif', 'Female', '9', '67890', 'Gynecology', '05006666666', 'elif@gmail.com', '66666666666'),
('Dr. Can', 'Male', '5', '78901', 'Psychiatry', '05007777777', 'can@gmail.com', '77777777777'),
('Dr. Hülya', 'Female', '14', '89012', 'Surgery', '05008888888', 'hulya@gmail.com', '88888888888'),
('Dr. Murat', 'Male', '11', '90123', 'Radiology', '05009999999', 'murat@gmail.com', '99999999999'),
('Dr. Zeynep', 'Female', '7', '01234', 'Endocrinology', '05000000000', 'zeynep@gmail.com', '12345678901');



INSERT INTO Patient (PName, PAddress, PBirthDate, PPhone, PGen, BloodGroup, MajorDisease, Email, NationalID, AssignedDoctorID)
VALUES 
('Hasan Yýlmaz', 'Ankara', '1980-01-01', '05011111111', 'Male', 'A+', 'Diabetes', 'hasan@gmail.com', '11112222333', 1000),
('Ayþe Kaya', 'Ýstanbul', '1990-05-15', '05012222222', 'Female', 'B-', 'Hypertension', 'aysek@gmail.com', '22223333444', 1001),
('Mehmet Can', 'Ýzmir', '1975-03-10', '05013333333', 'Male', 'O+', 'Heart Disease', 'mehmetc@gmail.com', '33334444555', 1002),
('Fatma Arslan', 'Antalya', '1985-12-20', '05014444444', 'Female', 'AB-', 'Asthma', 'fatma@gmail.com', '44445555666', 1003),
('Ali Demir', 'Bursa', '1995-07-25', '05015555555', 'Male', 'A-', 'Cancer', 'alid@gmail.com', '55556666777', 1004),
('Elif Þahin', 'Mersin', '2000-08-30', '05016666666', 'Female', 'B+', 'Allergy', 'elif@gmail.com', '66667777888', 1005),
('Hülya Kurt', 'Trabzon', '1992-04-14', '05017777777', 'Female', 'O-', 'Kidney Disease', 'hulya@gmail.com', '77778888999', 1006),
('Can Polat', 'Samsun', '1988-02-28', '05018888888', 'Male', 'A+', 'Diabetes', 'canp@gmail.com', '88889999000', 1007),
('Zeynep Uzun', 'Eskiþehir', '1999-11-11', '05019999999', 'Female', 'AB+', 'Hypertension', 'zeynepu@gmail.com', '99990000111', 1008),
('Murat Baþ', 'Konya', '1983-09-09', '05020000000', 'Male', 'O+', 'Heart Disease', 'muratb@gmail.com', '12345678011', 1009);


INSERT INTO Diagnosis (PatientID, DoctorID, PatientName, Symptoms, DiagnosticTest, Medicines)
VALUES 
(1, 1000, 'Hasan Yýlmaz', 'Cough', 'Blood Test', 'Paracetamol'),
(2, 1001, 'Ayþe Kaya', 'Fever', 'X-Ray', 'Ibuprofen'),
(3, 1002, 'Mehmet Can', 'Headache', 'MRI', 'Aspirin'),
(4, 1003, 'Fatma Arslan', 'Shortness of breath', 'Spirometry', 'Inhaler'),
(5, 1004, 'Ali Demir', 'Chest Pain', 'ECG', 'Nitroglycerin'),
(6, 1005, 'Elif Þahin', 'Rash', 'Allergy Test', 'Antihistamine'),
(7, 1006, 'Hülya Kurt', 'Nausea', 'Ultrasound', 'Omeprazole'),
(8, 1007, 'Can Polat', 'Back Pain', 'CT Scan', 'Muscle Relaxants'),
(9, 1008, 'Zeynep Uzun', 'Fatigue', 'Blood Test', 'Vitamin Supplements'),
(10, 1009, 'Murat Baþ', 'Weight Loss', 'Thyroid Test', 'Levothyroxine');
