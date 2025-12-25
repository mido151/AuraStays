    -- =====================================================
-- HOTEL MANAGEMENT SYSTEM - SAMPLE DATA INSERT SCRIPT
-- =====================================================
-- Run this AFTER creating the database schema
-- This script inserts realistic sample data for testing
-- =====================================================

USE HotelManagementDB;
GO

-- Clear existing data (if any)
PRINT '?? Cleaning existing data...';
DELETE FROM Bill;
DELETE FROM Payment;
DELETE FROM Review;
DELETE FROM Room;
DELETE FROM Reservation;
DELETE FROM Guest;
DELETE FROM Staff;
DELETE FROM Department;
DELETE FROM Hotel;
-- Users table is not cleared (keep admin user)
GO

-- Reset identity seeds
DBCC CHECKIDENT ('Bill', RESEED, 0);
DBCC CHECKIDENT ('Payment', RESEED, 0);
DBCC CHECKIDENT ('Review', RESEED, 0);
DBCC CHECKIDENT ('Room', RESEED, 0);
DBCC CHECKIDENT ('Reservation', RESEED, 0);
DBCC CHECKIDENT ('Guest', RESEED, 0);
DBCC CHECKIDENT ('Staff', RESEED, 0);
DBCC CHECKIDENT ('Department', RESEED, 0);
DBCC CHECKIDENT ('Hotel', RESEED, 0);
GO

PRINT '? Cleanup complete. Starting data insertion...';
GO

-- =====================================================
-- 1. INSERT HOTELS
-- =====================================================
PRINT '?? Inserting Hotels...';

INSERT INTO Hotel (Name, Address, City, Country, PostalCode, Phone, Rating, ImageUrl, Description) VALUES
('Grand Luxury Paris', '123 Avenue des Champs-Élysées', 'Paris', 'France', '75008', '+33-1-23-45-67-89', 4.8, 
 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800', 
 'Experience ultimate luxury in the heart of Paris. Our 5-star hotel offers breathtaking views of the Eiffel Tower, world-class dining, and unparalleled service.'),

('Azure Maldives Resort', '456 Beach Road, Malé Atoll', 'Malé', 'Maldives', '20026', '+960-664-1234', 4.9, 
 'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800', 
 'Paradise awaits at our overwater villas. Crystal clear waters, pristine beaches, and luxury accommodations create an unforgettable tropical escape.'),

('Kyoto Traditional Inn', '789 Gion District, Higashiyama', 'Kyoto', 'Japan', '605-0001', '+81-75-123-4567', 5.0, 
 'https://images.unsplash.com/photo-1545569341-9eb8b30979d9?w=800', 
 'Immerse yourself in Japanese culture at our traditional ryokan. Experience authentic tea ceremonies, zen gardens, and exquisite kaiseki cuisine.'),

('Manhattan Sky Tower', '321 5th Avenue', 'New York', 'USA', '10001', '+1-212-555-0100', 4.7, 
 'https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800', 
 'Soar above New York City in our luxury skyscraper hotel. Panoramic views, rooftop bar, and steps away from Times Square and Central Park.'),

('Swiss Alpine Lodge', '654 Mountain Road', 'Zermatt', 'Switzerland', '3920', '+41-27-123-4567', 4.9, 
 'https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800', 
 'Nestled at the foot of the Matterhorn, our alpine retreat offers world-class skiing, spa treatments, and cozy Swiss hospitality.'),

('Santorini Sunset Villa', '987 Caldera View, Oia', 'Santorini', 'Greece', '84700', '+30-22860-12345', 4.8, 
 'https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800', 
 'Witness the most beautiful sunsets from our cliffside villas. White-washed architecture, infinity pools, and stunning Aegean Sea views.'),

('Dubai Pearl Hotel', '147 Sheikh Zayed Road', 'Dubai', 'UAE', '00000', '+971-4-123-4567', 4.6, 
 'https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800', 
 'Modern opulence meets Arabian hospitality. Luxury shopping, indoor skiing, and breathtaking views of the Burj Khalifa.'),

('Bali Paradise Resort', '258 Ubud Monkey Forest Road', 'Ubud', 'Indonesia', '80571', '+62-361-123456', 4.9, 
 'https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800', 
 'Find your zen in the heart of Bali. Jungle views, yoga retreats, rice terrace vistas, and traditional Balinese spa treatments.');
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' hotels';
GO

-- =====================================================
-- 2. INSERT DEPARTMENTS
-- =====================================================
PRINT '?? Inserting Departments...';

INSERT INTO Department (Name, Hotel_ID, Head_Staff_ID) VALUES
-- Grand Luxury Paris (Hotel 1)
('Front Desk', 1, NULL),
('Housekeeping', 1, NULL),
('Restaurant & Bar', 1, NULL),
('Concierge', 1, NULL),

-- Azure Maldives Resort (Hotel 2)
('Front Desk', 2, NULL),
('Spa & Wellness', 2, NULL),
('Water Sports', 2, NULL),

-- Kyoto Traditional Inn (Hotel 3)
('Reception', 3, NULL),
('Tea House', 3, NULL),

-- Manhattan Sky Tower (Hotel 4)
('Front Desk', 4, NULL),
('Security', 4, NULL),
('Rooftop Bar', 4, NULL),

-- Swiss Alpine Lodge (Hotel 5)
('Reception', 5, NULL),
('Ski Equipment', 5, NULL),

-- Santorini Sunset Villa (Hotel 6)
('Guest Services', 6, NULL),

-- Dubai Pearl Hotel (Hotel 7)
('Front Desk', 7, NULL),
('Valet Services', 7, NULL),

-- Bali Paradise Resort (Hotel 8)
('Front Desk', 8, NULL),
('Yoga Studio', 8, NULL);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' departments';
GO

-- =====================================================
-- 3. INSERT STAFF
-- =====================================================
PRINT '?? Inserting Staff...';

INSERT INTO Staff (First_Name, Last_Name, DOB, Gender, Email, Salary, Dept_ID, Hotel_ID, Position, IsActive) VALUES
-- Grand Luxury Paris Staff
('Marie', 'Dubois', '1985-03-15', 'Female', 'marie.dubois@grandluxury.com', 55000.00, 1, 1, 'Front Desk Manager', 1),
('Pierre', 'Laurent', '1990-07-22', 'Male', 'pierre.laurent@grandluxury.com', 42000.00, 2, 1, 'Housekeeping Supervisor', 1),
('Sophie', 'Martin', '1988-11-30', 'Female', 'sophie.martin@grandluxury.com', 48000.00, 3, 1, 'Head Chef', 1),
('Jean', 'Renard', '1992-05-18', 'Male', 'jean.renard@grandluxury.com', 38000.00, 4, 1, 'Chief Concierge', 1),

-- Azure Maldives Resort Staff
('Ahmed', 'Hassan', '1987-05-10', 'Male', 'ahmed.hassan@azuremaldives.com', 52000.00, 5, 2, 'Guest Relations Manager', 1),
('Aisha', 'Mohamed', '1992-08-18', 'Female', 'aisha.mohamed@azuremaldives.com', 45000.00, 6, 2, 'Spa Director', 1),
('Ibrahim', 'Ali', '1989-12-03', 'Male', 'ibrahim.ali@azuremaldives.com', 40000.00, 7, 2, 'Water Sports Coordinator', 1),

-- Kyoto Traditional Inn Staff
('Yuki', 'Tanaka', '1986-02-14', 'Female', 'yuki.tanaka@kyotoinn.jp', 58000.00, 8, 3, 'Ryokan Manager', 1),
('Kenji', 'Yamamoto', '1991-09-25', 'Male', 'kenji.yamamoto@kyotoinn.jp', 46000.00, 9, 3, 'Tea Master', 1),

-- Manhattan Sky Tower Staff
('John', 'Smith', '1989-04-20', 'Male', 'john.smith@manhattansky.com', 62000.00, 10, 4, 'Hotel Director', 1),
('Emily', 'Johnson', '1993-12-05', 'Female', 'emily.johnson@manhattansky.com', 48000.00, 11, 4, 'Security Chief', 1),
('Michael', 'Davis', '1988-07-15', 'Male', 'michael.davis@manhattansky.com', 52000.00, 12, 4, 'Bar Manager', 1),

-- Swiss Alpine Lodge Staff
('Hans', 'Mueller', '1984-06-12', 'Male', 'hans.mueller@swissalpine.ch', 60000.00, 13, 5, 'Lodge Manager', 1),
('Anna', 'Schmidt', '1990-11-28', 'Female', 'anna.schmidt@swissalpine.ch', 44000.00, 14, 5, 'Ski Instructor', 1),

-- Santorini Sunset Villa Staff
('Dimitrios', 'Papadopoulos', '1987-03-09', 'Male', 'dimitrios@santorinisunet.gr', 50000.00, 15, 6, 'Villa Manager', 1),

-- Dubai Pearl Hotel Staff
('Fatima', 'Al-Maktoum', '1991-08-22', 'Female', 'fatima@dubaipearl.ae', 58000.00, 16, 7, 'Guest Services Director', 1),
('Omar', 'Abdullah', '1989-01-17', 'Male', 'omar@dubaipearl.ae', 42000.00, 17, 7, 'Head Valet', 1),

-- Bali Paradise Resort Staff
('Made', 'Wayan', '1986-04-30', 'Male', 'made.wayan@baliparadise.id', 48000.00, 18, 8, 'Resort Manager', 1),
('Ketut', 'Sari', '1993-10-12', 'Female', 'ketut.sari@baliparadise.id', 40000.00, 19, 8, 'Yoga Instructor', 1);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' staff members';
GO

-- Update Department Heads
UPDATE Department SET Head_Staff_ID = 1 WHERE Dept_ID = 1;
UPDATE Department SET Head_Staff_ID = 2 WHERE Dept_ID = 2;
UPDATE Department SET Head_Staff_ID = 3 WHERE Dept_ID = 3;
UPDATE Department SET Head_Staff_ID = 4 WHERE Dept_ID = 4;
UPDATE Department SET Head_Staff_ID = 5 WHERE Dept_ID = 5;
UPDATE Department SET Head_Staff_ID = 6 WHERE Dept_ID = 6;
UPDATE Department SET Head_Staff_ID = 7 WHERE Dept_ID = 7;
UPDATE Department SET Head_Staff_ID = 8 WHERE Dept_ID = 8;
UPDATE Department SET Head_Staff_ID = 9 WHERE Dept_ID = 9;
UPDATE Department SET Head_Staff_ID = 10 WHERE Dept_ID = 10;
UPDATE Department SET Head_Staff_ID = 11 WHERE Dept_ID = 11;
UPDATE Department SET Head_Staff_ID = 12 WHERE Dept_ID = 12;
UPDATE Department SET Head_Staff_ID = 13 WHERE Dept_ID = 13;
UPDATE Department SET Head_Staff_ID = 14 WHERE Dept_ID = 14;
UPDATE Department SET Head_Staff_ID = 15 WHERE Dept_ID = 15;
UPDATE Department SET Head_Staff_ID = 16 WHERE Dept_ID = 16;
UPDATE Department SET Head_Staff_ID = 17 WHERE Dept_ID = 17;
UPDATE Department SET Head_Staff_ID = 18 WHERE Dept_ID = 18;
UPDATE Department SET Head_Staff_ID = 19 WHERE Dept_ID = 19;
GO

PRINT '? Updated department heads';
GO

-- =====================================================
-- 4. INSERT GUESTS
-- =====================================================
PRINT '?? Inserting Guests...';

INSERT INTO Guest (First_Name, Last_Name, DOB, Nationality, Gender, Address_Street, Address_City, Address_Country, Loyalty_ID, UserId) VALUES
('James', 'Wilson', '1982-03-15', 'American', 'Male', '123 Main Street', 'New York', 'USA', 'GOLD001', NULL),
('Emma', 'Thompson', '1990-07-22', 'British', 'Female', '456 Oxford Road', 'London', 'UK', 'GOLD002', NULL),
('Mohammed', 'Al-Rashid', '1985-11-30', 'Emirati', 'Male', '789 Sheikh Zayed Road', 'Dubai', 'UAE', 'PLAT001', NULL),
('Sakura', 'Kobayashi', '1988-05-10', 'Japanese', 'Female', '321 Shibuya Street', 'Tokyo', 'Japan', 'GOLD003', NULL),
('Lucas', 'Dubois', '1992-08-18', 'French', 'Male', '654 Rue de Rivoli', 'Paris', 'France', 'SILVER001', NULL),
('Isabella', 'Rossi', '1986-02-14', 'Italian', 'Female', '987 Via Roma', 'Rome', 'Italy', 'GOLD004', NULL),
('Carlos', 'Garcia', '1991-09-25', 'Spanish', 'Male', '147 Gran Via', 'Madrid', 'Spain', 'SILVER002', NULL),
('Mei', 'Chen', '1989-04-20', 'Chinese', 'Female', '258 Nanjing Road', 'Shanghai', 'China', 'PLAT002', NULL),
('Oliver', 'Schmidt', '1993-12-05', 'German', 'Male', '369 Unter den Linden', 'Berlin', 'Germany', 'GOLD005', NULL),
('Sofia', 'Andersson', '1987-06-12', 'Swedish', 'Female', '741 Drottninggatan', 'Stockholm', 'Sweden', 'SILVER003', NULL),
('Raj', 'Patel', '1984-09-08', 'Indian', 'Male', '852 MG Road', 'Mumbai', 'India', 'GOLD006', NULL),
('Maria', 'Santos', '1995-03-27', 'Brazilian', 'Female', '963 Avenida Paulista', 'São Paulo', 'Brazil', 'SILVER004', NULL);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' guests';
GO

-- =====================================================
-- 5. INSERT ROOMS
-- =====================================================
PRINT '?? Inserting Rooms...';

INSERT INTO Room (Room_Number, Room_Type, Room_Price, Room_Status, Hotel_ID, Reservation_ID, ImageUrl, Capacity, Size) VALUES
-- Grand Luxury Paris (Hotel 1)
('101', 'Deluxe King', 350.00, 'Available', 1, NULL, 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800', 2, 35.00),
('102', 'Deluxe King', 350.00, 'Available', 1, NULL, 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800', 2, 35.00),
('201', 'Eiffel Suite', 850.00, 'Available', 1, NULL, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800', 4, 75.00),
('301', 'Presidential Suite', 2500.00, 'Available', 1, NULL, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800', 6, 150.00),

-- Azure Maldives Resort (Hotel 2)
('V01', 'Overwater Villa', 650.00, 'Available', 2, NULL, 'https://images.unsplash.com/photo-1540541338287-41700207dee6?w=800', 2, 55.00),
('V02', 'Overwater Villa', 650.00, 'Available', 2, NULL, 'https://images.unsplash.com/photo-1540541338287-41700207dee6?w=800', 2, 55.00),
('B01', 'Beach Villa', 550.00, 'Available', 2, NULL, 'https://images.unsplash.com/photo-1573052905904-34ad8c27f0cc?w=800', 3, 65.00),
('S01', 'Sunset Villa', 950.00, 'Available', 2, NULL, 'https://images.unsplash.com/photo-1602002418082-a4443e081dd1?w=800', 4, 85.00),

-- Kyoto Traditional Inn (Hotel 3)
('T01', 'Tatami Room', 420.00, 'Available', 3, NULL, 'https://images.unsplash.com/photo-1566195992011-5f6b21e539aa?w=800', 2, 30.00),
('T02', 'Tatami Room', 420.00, 'Available', 3, NULL, 'https://images.unsplash.com/photo-1566195992011-5f6b21e539aa?w=800', 2, 30.00),
('G01', 'Garden Suite', 680.00, 'Available', 3, NULL, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800', 3, 50.00),

-- Manhattan Sky Tower (Hotel 4)
('1501', 'Executive Room', 450.00, 'Available', 4, NULL, 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800', 2, 40.00),
('2001', 'Skyline Suite', 950.00, 'Available', 4, NULL, 'https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800', 4, 70.00),
('3001', 'Penthouse', 1800.00, 'Available', 4, NULL, 'https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800', 6, 120.00),

-- Swiss Alpine Lodge (Hotel 5)
('M01', 'Mountain View', 580.00, 'Available', 5, NULL, 'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800', 2, 42.00),
('M02', 'Mountain View', 580.00, 'Available', 5, NULL, 'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800', 2, 42.00),
('C01', 'Chalet Suite', 1200.00, 'Available', 5, NULL, 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800', 5, 90.00),

-- Santorini Sunset Villa (Hotel 6)
('S01', 'Caldera View Villa', 720.00, 'Available', 6, NULL, 'https://images.unsplash.com/photo-1602002418082-a4443e081dd1?w=800', 2, 48.00),
('S02', 'Infinity Pool Villa', 980.00, 'Available', 6, NULL, 'https://images.unsplash.com/photo-1512918728675-ed5a9ecdebfd?w=800', 3, 65.00),

-- Dubai Pearl Hotel (Hotel 7)
('F15', 'Deluxe Room', 400.00, 'Available', 7, NULL, 'https://images.unsplash.com/photo-1631049552057-403cdb8f0658?w=800', 2, 38.00),
('F25', 'Executive Suite', 850.00, 'Available', 7, NULL, 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800', 4, 68.00),

-- Bali Paradise Resort (Hotel 8)
('R01', 'Rice Terrace Room', 380.00, 'Available', 8, NULL, 'https://images.unsplash.com/photo-1455587734955-081b22074882?w=800', 2, 35.00),
('J01', 'Jungle Villa', 620.00, 'Available', 8, NULL, 'https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800', 3, 55.00),
('P01', 'Pool Villa', 820.00, 'Available', 8, NULL, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800', 4, 75.00);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' rooms';
GO

-- =====================================================
-- 6. INSERT RESERVATIONS
-- =====================================================
PRINT '?? Inserting Reservations...';

INSERT INTO Reservation (Booking_Date, CheckIn_Date, CheckOut_Date, Num_Guests, Status, Guest_ID, Created_By_Staff_ID) VALUES
-- Past reservations (Completed)
('2024-01-15 10:30:00', '2024-02-10', '2024-02-15', 2, 'Completed', 1, 1),
('2024-01-20 14:15:00', '2024-02-20', '2024-02-27', 2, 'Completed', 2, 5),
('2024-02-01 09:45:00', '2024-03-01', '2024-03-05', 2, 'Completed', 3, 8),

-- Current reservations (Confirmed - checked in)
('2024-11-20 16:20:00', '2024-12-15', '2024-12-25', 4, 'Confirmed', 4, 10),
('2024-11-25 11:00:00', '2024-12-18', '2024-12-28', 2, 'Confirmed', 5, 13),

-- Future reservations (Confirmed - not checked in yet)
('2024-12-01 13:30:00', '2025-01-10', '2025-01-17', 3, 'Confirmed', 6, 15),
('2024-12-05 15:45:00', '2025-01-15', '2025-01-25', 2, 'Confirmed', 7, 16),
('2024-12-10 10:00:00', '2025-02-01', '2025-02-10', 4, 'Confirmed', 8, 18),

-- Pending reservations
('2024-12-20 12:30:00', '2025-02-14', '2025-02-21', 2, 'Pending', 9, 1),
('2024-12-21 14:00:00', '2025-03-01', '2025-03-08', 3, 'Pending', 10, 5);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' reservations';
GO

-- Update rooms with active reservations
UPDATE Room SET Reservation_ID = 4, Room_Status = 'Occupied' WHERE Room_ID = 12; -- Manhattan for Guest 4
UPDATE Room SET Reservation_ID = 5, Room_Status = 'Occupied' WHERE Room_ID = 15; -- Swiss Alpine for Guest 5
GO

PRINT '? Updated room occupancy status';
GO

-- =====================================================
-- 7. INSERT PAYMENTS
-- =====================================================
PRINT '?? Inserting Payments...';

INSERT INTO Payment (Payment_Date, Amount, Method, Currency, Guest_ID, Reservation_ID, Staff_ID) VALUES
-- Completed payments
('2024-02-10 14:00:00', 1750.00, 'Credit Card', 'EUR', 1, 1, 1),
('2024-02-20 15:30:00', 4550.00, 'Credit Card', 'USD', 2, 2, 5),
('2024-03-01 12:00:00', 2720.00, 'Debit Card', 'AED', 3, 3, 8),

-- Current reservation payments
('2024-12-15 10:30:00', 9500.00, 'Bank Transfer', 'USD', 4, 4, 10),
('2024-12-18 11:15:00', 5800.00, 'Credit Card', 'CHF', 5, 5, 13),

-- Future reservation payments (deposits)
('2024-12-01 13:45:00', 4760.00, 'Credit Card', 'EUR', 6, 6, 15),
('2024-12-05 14:20:00', 7840.00, 'Credit Card', 'AED', 7, 7, 16),
('2024-12-10 16:00:00', 7650.00, 'Bank Transfer', 'USD', 8, 8, 18),

-- Pending payments
('2024-12-20 12:30:00', 0.00, 'Pending', 'USD', 9, 9, 1),
('2024-12-21 14:00:00', 0.00, 'Pending', 'USD', 10, 10, 5);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' payments';
GO

-- =====================================================
-- 8. INSERT BILLS
-- =====================================================
PRINT '?? Inserting Bills...';

INSERT INTO Bill (Bill_Date, Total_Amount, Payment_ID, Reservation_ID) VALUES
('2024-02-15 11:00:00', 1750.00, 1, 1),
('2024-02-27 10:00:00', 4550.00, 2, 2),
('2024-03-05 12:00:00', 2720.00, 3, 3),
('2024-12-15 10:30:00', 9500.00, 4, 4),
('2024-12-18 11:15:00', 5800.00, 5, 5),
('2024-12-01 13:45:00', 4760.00, 6, 6);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' bills';
GO

-- =====================================================
-- 9. INSERT REVIEWS
-- =====================================================
PRINT '? Inserting Reviews...';

INSERT INTO Review (HotelId, GuestId, Rating, Comment, CreatedAt, IsApproved) VALUES
-- Approved reviews
(1, 1, 5, 'Absolutely stunning hotel! The Eiffel Tower view from our room was breathtaking. Staff was incredibly attentive and the breakfast was divine.', '2024-02-16 14:30:00', 1),
(2, 2, 5, 'Paradise on earth! The overwater villa was a dream come true. Crystal clear waters, excellent service, and the spa was incredible.', '2024-02-28 16:45:00', 1),
(3, 3, 5, 'An authentic Japanese experience. The traditional tea ceremony was unforgettable. The attention to detail and hospitality was outstanding.', '2024-03-06 10:20:00', 1),

-- Pending reviews (not approved yet)
(4, 4, 4, 'Great location in the heart of Manhattan. The rooftop bar has amazing views. Only downside was some noise from the street at night.', '2024-12-23 20:15:00', 0),
(5, 5, 5, 'Perfect winter getaway! Skiing was fantastic, the chalet was cozy, and the Swiss fondue was the best I have ever had.', '2024-12-23 18:30:00', 0);
GO

PRINT '? Inserted ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' reviews';
GO

-- =====================================================
-- 10. INSERT ADDITIONAL USERS (Hotel Admins & Customers)
-- =====================================================
PRINT '?? Inserting Additional Users...';

-- Hotel Admin for Grand Luxury Paris
INSERT INTO Users (Username, Email, PasswordHash, Role, HotelId, CreatedAt, IsActive)
VALUES ('paris_admin', 'admin@grandluxury.com', 
        '$2a$11$rqYv5pLPJQKZFCfPqQKiE.Zg0n8mVLJKUq.POQmJu.2QmqFZvPgJi', 
        'HotelAdmin', 1, GETDATE(), 1);



