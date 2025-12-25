-- =====================================================
-- NUCLEAR OPTION - COMPLETE DATABASE RESET
-- =====================================================

USE master;
GO

-- Step 1: Try to set to multi-user
BEGIN TRY
    ALTER DATABASE HotelManagementDB SET MULTI_USER WITH ROLLBACK IMMEDIATE;
    PRINT '? Set to multi-user';
END TRY
BEGIN CATCH
    PRINT '?? Database might not exist or already in correct state';
END CATCH
GO

-- Step 2: Kill all connections using cursor
DECLARE @session_id INT;
DECLARE @sql NVARCHAR(MAX);

DECLARE session_cursor CURSOR FOR
    SELECT session_id
    FROM sys.dm_exec_sessions
    WHERE database_id = DB_ID('HotelManagementDB')
      AND session_id <> @@SPID;

OPEN session_cursor;
FETCH NEXT FROM session_cursor INTO @session_id;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = 'KILL ' + CAST(@session_id AS NVARCHAR(10));
    BEGIN TRY
        EXEC sp_executesql @sql;
        PRINT '? Killed session: ' + CAST(@session_id AS NVARCHAR(10));
    END TRY
    BEGIN CATCH
        PRINT '?? Could not kill session: ' + CAST(@session_id AS NVARCHAR(10));
    END CATCH
    
    FETCH NEXT FROM session_cursor INTO @session_id;
END

CLOSE session_cursor;
DEALLOCATE session_cursor;
GO

-- Step 3: Wait a moment
WAITFOR DELAY '00:00:03';
GO

-- Step 4: Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelManagementDB')
BEGIN
    DROP DATABASE HotelManagementDB;
    PRINT '? Database dropped';
END
GO

-- Step 5: Create fresh database
CREATE DATABASE HotelManagementDB;
GO

USE HotelManagementDB;
GO

PRINT '??? SUCCESS! Fresh database created!';
PRINT '?? Now run the table creation script below...';
GO

-- =====================================================
-- NOW CREATE ALL TABLES
-- =====================================================

-- Table: Users
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Customer',
    HotelId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Table: Hotel
CREATE TABLE Hotel (
    Hotel_ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20),
    Phone NVARCHAR(20),
    Rating DECIMAL(3,2) CHECK (Rating >= 0 AND Rating <= 5),
    ImageUrl NVARCHAR(500),
    Description NVARCHAR(MAX)
);
GO

-- Table: Department
CREATE TABLE Department (
    Dept_ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Hotel_ID INT NOT NULL,
    Head_Staff_ID INT,
    CONSTRAINT FK_Department_Hotel FOREIGN KEY (Hotel_ID) REFERENCES Hotel(Hotel_ID)
);
GO

-- Table: Staff
CREATE TABLE Staff (
    Staff_ID INT PRIMARY KEY IDENTITY(1,1),
    First_Name NVARCHAR(100) NOT NULL,
    Last_Name NVARCHAR(100) NOT NULL,
    DOB DATE,
    Gender NVARCHAR(10),
    Email NVARCHAR(255),
    Salary DECIMAL(10,2),
    Dept_ID INT,
    Hotel_ID INT NOT NULL,
    Position NVARCHAR(100),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Staff_Department FOREIGN KEY (Dept_ID) REFERENCES Department(Dept_ID),
    CONSTRAINT FK_Staff_Hotel FOREIGN KEY (Hotel_ID) REFERENCES Hotel(Hotel_ID)
);
GO

ALTER TABLE Department
ADD CONSTRAINT FK_Department_Head_Staff FOREIGN KEY (Head_Staff_ID) REFERENCES Staff(Staff_ID);
GO

-- Table: Guest
CREATE TABLE Guest (
    Guest_ID INT PRIMARY KEY IDENTITY(1,1),
    First_Name NVARCHAR(100) NOT NULL,
    Last_Name NVARCHAR(100) NOT NULL,
    DOB DATE,
    Nationality NVARCHAR(100),
    Gender NVARCHAR(10),
    Address_Street NVARCHAR(500),
    Address_City NVARCHAR(100),
    Address_Country NVARCHAR(100),
    Loyalty_ID NVARCHAR(50),
    UserId INT NULL,
    CONSTRAINT FK_Guest_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

-- Table: Reservation
CREATE TABLE Reservation (
    Reservation_ID INT PRIMARY KEY IDENTITY(1,1),
    Booking_Date DATETIME NOT NULL DEFAULT GETDATE(),
    CheckIn_Date DATE NOT NULL,
    CheckOut_Date DATE NOT NULL,
    Num_Guests INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    Guest_ID INT NOT NULL,
    Created_By_Staff_ID INT,
    CONSTRAINT FK_Reservation_Guest FOREIGN KEY (Guest_ID) REFERENCES Guest(Guest_ID),
    CONSTRAINT FK_Reservation_Staff FOREIGN KEY (Created_By_Staff_ID) REFERENCES Staff(Staff_ID),
    CONSTRAINT CHK_CheckOut_After_CheckIn CHECK (CheckOut_Date > CheckIn_Date)
);
GO

-- Table: Room
CREATE TABLE Room (
    Room_ID INT PRIMARY KEY IDENTITY(1,1),
    Room_Number NVARCHAR(20) NOT NULL,
    Room_Type NVARCHAR(50) NOT NULL,
    Room_Price DECIMAL(10,2) NOT NULL,
    Room_Status NVARCHAR(20) NOT NULL DEFAULT 'Available',
    Hotel_ID INT NOT NULL,
    Reservation_ID INT,
    ImageUrl NVARCHAR(500),
    Capacity INT,
    Size DECIMAL(10,2),
    CONSTRAINT FK_Room_Hotel FOREIGN KEY (Hotel_ID) REFERENCES Hotel(Hotel_ID),
    CONSTRAINT FK_Room_Reservation FOREIGN KEY (Reservation_ID) REFERENCES Reservation(Reservation_ID)
);
GO

-- Table: Payment
CREATE TABLE Payment (
    Payment_ID INT PRIMARY KEY IDENTITY(1,1),
    Payment_Date DATETIME NOT NULL DEFAULT GETDATE(),
    Amount DECIMAL(10,2) NOT NULL,
    Method NVARCHAR(50) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'USD',
    Guest_ID INT NOT NULL,
    Reservation_ID INT NOT NULL,
    Staff_ID INT,
    CONSTRAINT FK_Payment_Guest FOREIGN KEY (Guest_ID) REFERENCES Guest(Guest_ID),
    CONSTRAINT FK_Payment_Reservation FOREIGN KEY (Reservation_ID) REFERENCES Reservation(Reservation_ID),
    CONSTRAINT FK_Payment_Staff FOREIGN KEY (Staff_ID) REFERENCES Staff(Staff_ID)
);
GO

-- Table: Bill
CREATE TABLE Bill (
    Bill_ID INT PRIMARY KEY IDENTITY(1,1),
    Bill_Date DATETIME NOT NULL DEFAULT GETDATE(),
    Total_Amount DECIMAL(10,2) NOT NULL,
    Payment_ID INT NOT NULL,
    Reservation_ID INT NOT NULL,
    CONSTRAINT FK_Bill_Payment FOREIGN KEY (Payment_ID) REFERENCES Payment(Payment_ID),
    CONSTRAINT FK_Bill_Reservation FOREIGN KEY (Reservation_ID) REFERENCES Reservation(Reservation_ID)
);
GO

-- Table: Review
CREATE TABLE Review (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    GuestId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsApproved BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Review_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(Hotel_ID),
    CONSTRAINT FK_Review_Guest FOREIGN KEY (GuestId) REFERENCES Guest(Guest_ID)
);
GO

-- Create Indexes
CREATE INDEX IX_Room_Hotel ON Room(Hotel_ID);
CREATE INDEX IX_Room_Reservation ON Room(Reservation_ID);
CREATE INDEX IX_Reservation_Guest ON Reservation(Guest_ID);
CREATE INDEX IX_Reservation_Dates ON Reservation(CheckIn_Date, CheckOut_Date);
CREATE INDEX IX_Payment_Reservation ON Payment(Reservation_ID);
CREATE INDEX IX_Staff_Hotel ON Staff(Hotel_ID);
GO

-- Insert Admin User (Password: Admin@123)
INSERT INTO Users (Username, Email, PasswordHash, Role, CreatedAt, IsActive)
VALUES ('admin', 'admin@aurastays.com', 
        '$2a$11$rqYv5pLPJQKZFCfPqQKiE.Zg0n8mVLJKUq.POQmJu.2QmqFZvPgJi', 
        'Admin', GETDATE(), 1);
GO

PRINT '??? ALL TABLES CREATED!';
PRINT '? Admin user created (admin / Admin@123)';
PRINT '?? Now run 02_InsertSampleData.sql';
GO