

-- Drop existing database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelManagementDB')
BEGIN
    ALTER DATABASE HotelManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HotelManagementDB;
END
GO

-- Create new database
CREATE DATABASE HotelManagementDB;
GO

USE HotelManagementDB;
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
    Rating DECIMAL(3,2) CHECK (Rating >= 0 AND Rating <= 5)
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
    CONSTRAINT FK_Staff_Department FOREIGN KEY (Dept_ID) REFERENCES Department(Dept_ID),
    CONSTRAINT FK_Staff_Hotel FOREIGN KEY (Hotel_ID) REFERENCES Hotel(Hotel_ID)
);
GO

-- Add foreign key for Department Head after Staff table is created
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
    Loyalty_ID NVARCHAR(50)
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
    CONSTRAINT FK_Room_Hotel FOREIGN KEY (Hotel_ID) REFERENCES Hotel(Hotel_ID)
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

-- Add foreign key for Room after Reservation table is created
ALTER TABLE Room
ADD CONSTRAINT FK_Room_Reservation FOREIGN KEY (Reservation_ID) REFERENCES Reservation(Reservation_ID);
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


-- Indexes for Performance

CREATE INDEX IX_Room_Hotel ON Room(Hotel_ID);
CREATE INDEX IX_Room_Reservation ON Room(Reservation_ID);
CREATE INDEX IX_Reservation_Guest ON Reservation(Guest_ID);
CREATE INDEX IX_Reservation_Dates ON Reservation(CheckIn_Date, CheckOut_Date);
CREATE INDEX IX_Payment_Reservation ON Payment(Reservation_ID);
CREATE INDEX IX_Payment_Guest ON Payment(Guest_ID);
CREATE INDEX IX_Bill_Reservation ON Bill(Reservation_ID);
CREATE INDEX IX_Staff_Hotel ON Staff(Hotel_ID);
CREATE INDEX IX_Staff_Department ON Staff(Dept_ID);
CREATE INDEX IX_Department_Hotel ON Department(Hotel_ID);
GO

PRINT 'Database schema created successfully!';
GO