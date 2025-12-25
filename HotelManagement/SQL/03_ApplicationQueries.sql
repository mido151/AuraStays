

USE HotelManagementDB;
GO

-- ================================================
-- DASHBOARD QUERIES
-- ================================================

-- Query 1: Get total count of hotels
SELECT COUNT(*) AS TotalHotels FROM Hotel;
GO

-- Query 2: Get total count of rooms
SELECT COUNT(*) AS TotalRooms FROM Room;
GO

-- Query 3: Get total count of available rooms
SELECT COUNT(*) AS AvailableRooms FROM Room WHERE Room_Status = 'Available';
GO

-- Query 4: Get total potential revenue (sum of all room prices)
SELECT SUM(Room_Price) AS TotalRevenue FROM Room;
GO

-- Query 5: Get dashboard statistics (combined)
SELECT 
    (SELECT COUNT(*) FROM Hotel) AS TotalHotels,
    (SELECT COUNT(*) FROM Room) AS TotalRooms,
    (SELECT COUNT(*) FROM Room WHERE Room_Status = 'Available') AS AvailableRooms,
    (SELECT COUNT(*) FROM Guest) AS TotalGuests,
    (SELECT COUNT(*) FROM Reservation WHERE Status = 'Confirmed') AS ConfirmedReservations,
    (SELECT COUNT(*) FROM Staff) AS TotalStaff;
GO

-- ================================================
-- HOTEL QUERIES
-- ================================================

-- Query 6: Get all hotels
SELECT Hotel_ID, Name, Address, City, Country, Phone, Rating
FROM Hotel
ORDER BY Rating DESC, Name;
GO

-- Query 7: Get hotel by ID
SELECT Hotel_ID, Name, Address, City, Country, PostalCode, Phone, Rating
FROM Hotel
WHERE Hotel_ID = 1;
GO

-- Query 8: Search hotels by name or city
DECLARE @SearchTerm NVARCHAR(200) = 'Paris';
SELECT Hotel_ID, Name, City, Country, Rating, Phone
FROM Hotel
WHERE Name LIKE '%' + @SearchTerm + '%' OR City LIKE '%' + @SearchTerm + '%'
ORDER BY Rating DESC;
GO

-- Query 9: Get top rated hotels
SELECT TOP 5 Hotel_ID, Name, City, Country, Rating, Phone
FROM Hotel
ORDER BY Rating DESC;
GO

-- Query 10: Get hotels by country
SELECT Hotel_ID, Name, City, Country, Rating
FROM Hotel
WHERE Country = 'France'
ORDER BY Rating DESC;
GO

-- Query 11: Get hotel with department count
SELECT 
    h.Hotel_ID,
    h.Name,
    h.City,
    h.Country,
    COUNT(d.Dept_ID) AS DepartmentCount
FROM Hotel h
LEFT JOIN Department d ON h.Hotel_ID = d.Hotel_ID
GROUP BY h.Hotel_ID, h.Name, h.City, h.Country
ORDER BY h.Name;
GO

-- ================================================
-- ROOM QUERIES
-- ================================================

-- Query 12: Get all rooms
SELECT Room_ID, Room_Number, Room_Type, Room_Price, Room_Status, Hotel_ID
FROM Room
ORDER BY Hotel_ID, Room_Number;
GO

-- Query 13: Get available rooms
SELECT Room_ID, Room_Number, Room_Type, Room_Price, Hotel_ID
FROM Room
WHERE Room_Status = 'Available'
ORDER BY Room_Price;
GO

-- Query 14: Get rooms by hotel ID
SELECT Room_ID, Room_Number, Room_Type, Room_Price, Room_Status
FROM Room
WHERE Hotel_ID = 1
ORDER BY Room_Number;
GO

-- Query 15: Get room details with hotel information
SELECT 
    r.Room_ID,
    r.Room_Number,
    r.Room_Type,
    r.Room_Price,
    r.Room_Status,
    h.Name AS HotelName,
    h.City,
    h.Country
FROM Room r
INNER JOIN Hotel h ON r.Hotel_ID = h.Hotel_ID
WHERE r.Room_ID = 1;
GO

-- Query 16: Get rooms by price range
DECLARE @MinPrice DECIMAL(10,2) = 300.00;
DECLARE @MaxPrice DECIMAL(10,2) = 600.00;
SELECT Room_ID, Room_Number, Room_Type, Room_Price, Hotel_ID
FROM Room
WHERE Room_Status = 'Available' 
  AND Room_Price BETWEEN @MinPrice AND @MaxPrice
ORDER BY Room_Price;
GO

-- Query 17: Get rooms by type
SELECT Room_ID, Room_Number, Room_Type, Room_Price, Room_Status, Hotel_ID
FROM Room
WHERE Room_Type = 'Deluxe King'
ORDER BY Room_Price;
GO

-- Query 18: Check room availability
SELECT 
    Room_ID,
    Room_Number,
    Room_Type,
    Room_Price,
    Room_Status,
    CASE 
        WHEN Room_Status = 'Available' THEN 'Yes'
        ELSE 'No'
    END AS IsAvailable
FROM Room
WHERE Room_ID = 1;
GO

-- ================================================
-- GUEST QUERIES
-- ================================================

-- Query 19: Get all guests
SELECT Guest_ID, First_Name, Last_Name, Nationality, Address_City, Address_Country, Loyalty_ID
FROM Guest
ORDER BY Last_Name, First_Name;
GO

-- Query 20: Get guest by ID
SELECT Guest_ID, First_Name, Last_Name, DOB, Nationality, Gender, 
       Address_Street, Address_City, Address_Country, Loyalty_ID
FROM Guest
WHERE Guest_ID = 1;
GO

-- Query 21: Search guest by name
DECLARE @GuestName NVARCHAR(100) = 'Wilson';
SELECT Guest_ID, First_Name, Last_Name, Nationality, Loyalty_ID
FROM Guest
WHERE First_Name LIKE '%' + @GuestName + '%' OR Last_Name LIKE '%' + @GuestName + '%'
ORDER BY Last_Name, First_Name;
GO

-- Query 22: Get guests by nationality
SELECT Guest_ID, First_Name, Last_Name, Address_City, Loyalty_ID
FROM Guest
WHERE Nationality = 'American'
ORDER BY Last_Name;
GO

-- Query 23: Get guest with loyalty ID
SELECT Guest_ID, First_Name, Last_Name, Nationality, Loyalty_ID
FROM Guest
WHERE Loyalty_ID = 'LOY001';
GO

-- ================================================
-- RESERVATION QUERIES
-- ================================================

-- Query 24: Get all reservations
SELECT 
    r.Reservation_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    r.CheckIn_Date,
    r.CheckOut_Date,
    r.Num_Guests,
    r.Status,
    r.Booking_Date
FROM Reservation r
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
ORDER BY r.Booking_Date DESC;
GO

-- Query 25: Get reservations by guest ID
SELECT 
    r.Reservation_ID,
    r.CheckIn_Date,
    r.CheckOut_Date,
    r.Num_Guests,
    r.Status,
    rm.Room_Number,
    h.Name AS HotelName
FROM Reservation r
LEFT JOIN Room rm ON r.Reservation_ID = rm.Reservation_ID
LEFT JOIN Hotel h ON rm.Hotel_ID = h.Hotel_ID
WHERE r.Guest_ID = 1
ORDER BY r.CheckIn_Date DESC;
GO

-- Query 26: Get upcoming reservations
SELECT 
    r.Reservation_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    r.CheckIn_Date,
    r.CheckOut_Date,
    r.Num_Guests,
    r.Status
FROM Reservation r
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
WHERE r.Status = 'Confirmed' AND r.CheckIn_Date >= CAST(GETDATE() AS DATE)
ORDER BY r.CheckIn_Date;
GO

-- Query 27: Get reservations by status
SELECT 
    r.Reservation_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    r.CheckIn_Date,
    r.CheckOut_Date,
    r.Status
FROM Reservation r
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
WHERE r.Status = 'Confirmed'
ORDER BY r.CheckIn_Date;
GO

-- Query 28: Get reservation details with room and hotel info
SELECT 
    r.Reservation_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    r.CheckIn_Date,
    r.CheckOut_Date,
    r.Num_Guests,
    r.Status,
    rm.Room_Number,
    rm.Room_Type,
    h.Name AS HotelName,
    h.City
FROM Reservation r
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
LEFT JOIN Room rm ON r.Reservation_ID = rm.Reservation_ID
LEFT JOIN Hotel h ON rm.Hotel_ID = h.Hotel_ID
WHERE r.Reservation_ID = 1;
GO

-- Query 29: Check room availability for specific dates
DECLARE @RoomID INT = 1;
DECLARE @CheckIn DATE = '2024-07-01';
DECLARE @CheckOut DATE = '2024-07-05';

SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Room rm
            INNER JOIN Reservation r ON rm.Reservation_ID = r.Reservation_ID
            WHERE rm.Room_ID = @RoomID 
              AND r.Status IN ('Confirmed', 'Pending')
              AND (
                  (@CheckIn BETWEEN r.CheckIn_Date AND r.CheckOut_Date) OR
                  (@CheckOut BETWEEN r.CheckIn_Date AND r.CheckOut_Date) OR
                  (r.CheckIn_Date BETWEEN @CheckIn AND @CheckOut)
              )
        ) THEN 'Not Available'
        ELSE 'Available'
    END AS AvailabilityStatus;
GO

-- ================================================
-- STAFF QUERIES
-- ================================================

-- Query 30: Get all staff
SELECT 
    s.Staff_ID,
    s.First_Name,
    s.Last_Name,
    s.Email,
    s.Salary,
    d.Name AS Department,
    h.Name AS Hotel
FROM Staff s
LEFT JOIN Department d ON s.Dept_ID = d.Dept_ID
INNER JOIN Hotel h ON s.Hotel_ID = h.Hotel_ID
ORDER BY h.Name, s.Last_Name;
GO

-- Query 31: Get staff by hotel ID
SELECT 
    s.Staff_ID,
    s.First_Name,
    s.Last_Name,
    s.Email,
    d.Name AS Department,
    s.Salary
FROM Staff s
LEFT JOIN Department d ON s.Dept_ID = d.Dept_ID
WHERE s.Hotel_ID = 1
ORDER BY s.Last_Name;
GO

-- Query 32: Get staff by department
SELECT 
    s.Staff_ID,
    s.First_Name,
    s.Last_Name,
    s.Email,
    s.Salary,
    h.Name AS Hotel
FROM Staff s
INNER JOIN Hotel h ON s.Hotel_ID = h.Hotel_ID
WHERE s.Dept_ID = 1
ORDER BY s.Last_Name;
GO

-- Query 33: Get department heads
SELECT 
    s.Staff_ID,
    s.First_Name,
    s.Last_Name,
    d.Name AS Department,
    h.Name AS Hotel
FROM Staff s
INNER JOIN Department d ON s.Staff_ID = d.Head_Staff_ID
INNER JOIN Hotel h ON s.Hotel_ID = h.Hotel_ID
ORDER BY h.Name, d.Name;
GO

-- ================================================
-- DEPARTMENT QUERIES
-- ================================================

-- Query 34: Get all departments
SELECT 
    d.Dept_ID,
    d.Name,
    h.Name AS Hotel,
    s.First_Name + ' ' + s.Last_Name AS HeadStaff
FROM Department d
INNER JOIN Hotel h ON d.Hotel_ID = h.Hotel_ID
LEFT JOIN Staff s ON d.Head_Staff_ID = s.Staff_ID
ORDER BY h.Name, d.Name;
GO

-- Query 35: Get departments by hotel
SELECT 
    d.Dept_ID,
    d.Name,
    s.First_Name + ' ' + s.Last_Name AS HeadStaff
FROM Department d
LEFT JOIN Staff s ON d.Head_Staff_ID = s.Staff_ID
WHERE d.Hotel_ID = 1
ORDER BY d.Name;
GO

-- Query 36: Get department with staff count
SELECT 
    d.Dept_ID,
    d.Name AS Department,
    h.Name AS Hotel,
    COUNT(s.Staff_ID) AS StaffCount
FROM Department d
INNER JOIN Hotel h ON d.Hotel_ID = h.Hotel_ID
LEFT JOIN Staff s ON d.Dept_ID = s.Dept_ID
GROUP BY d.Dept_ID, d.Name, h.Name
ORDER BY h.Name, d.Name;
GO

-- ================================================
-- PAYMENT QUERIES
-- ================================================

-- Query 37: Get all payments
SELECT 
    p.Payment_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    p.Payment_Date,
    p.Amount,
    p.Method,
    p.Currency,
    r.Reservation_ID
FROM Payment p
INNER JOIN Guest g ON p.Guest_ID = g.Guest_ID
INNER JOIN Reservation r ON p.Reservation_ID = r.Reservation_ID
ORDER BY p.Payment_Date DESC;
GO

-- Query 38: Get payments by guest ID
SELECT 
    p.Payment_ID,
    p.Payment_Date,
    p.Amount,
    p.Method,
    p.Currency,
    r.CheckIn_Date,
    r.CheckOut_Date
FROM Payment p
INNER JOIN Reservation r ON p.Reservation_ID = r.Reservation_ID
WHERE p.Guest_ID = 1
ORDER BY p.Payment_Date DESC;
GO

-- Query 39: Get payments by method
SELECT 
    p.Payment_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    p.Payment_Date,
    p.Amount,
    p.Currency
FROM Payment p
INNER JOIN Guest g ON p.Guest_ID = g.Guest_ID
WHERE p.Method = 'Credit Card'
ORDER BY p.Payment_Date DESC;
GO

-- Query 40: Get total revenue by hotel
SELECT 
    h.Name AS Hotel,
    SUM(p.Amount) AS TotalRevenue,
    COUNT(p.Payment_ID) AS PaymentCount
FROM Payment p
INNER JOIN Reservation r ON p.Reservation_ID = r.Reservation_ID
INNER JOIN Room rm ON r.Reservation_ID = rm.Reservation_ID
INNER JOIN Hotel h ON rm.Hotel_ID = h.Hotel_ID
WHERE p.Method != 'Pending'
GROUP BY h.Hotel_ID, h.Name
ORDER BY TotalRevenue DESC;
GO

-- ================================================
-- BILL QUERIES
-- ================================================

-- Query 41: Get all bills
SELECT 
    b.Bill_ID,
    b.Bill_Date,
    b.Total_Amount,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    r.Reservation_ID
FROM Bill b
INNER JOIN Reservation r ON b.Reservation_ID = r.Reservation_ID
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
ORDER BY b.Bill_Date DESC;
GO

-- Query 42: Get bill by ID
SELECT 
    b.Bill_ID,
    b.Bill_Date,
    b.Total_Amount,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    r.CheckIn_Date,
    r.CheckOut_Date,
    p.Method AS PaymentMethod
FROM Bill b
INNER JOIN Reservation r ON b.Reservation_ID = r.Reservation_ID
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
INNER JOIN Payment p ON b.Payment_ID = p.Payment_ID
WHERE b.Bill_ID = 1;
GO

-- Query 43: Get bills by date range
DECLARE @StartDate DATE = '2024-03-01';
DECLARE @EndDate DATE = '2024-06-30';

SELECT 
    b.Bill_ID,
    b.Bill_Date,
    b.Total_Amount,
    g.First_Name + ' ' + g.Last_Name AS GuestName
FROM Bill b
INNER JOIN Reservation r ON b.Reservation_ID = r.Reservation_ID
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
WHERE b.Bill_Date BETWEEN @StartDate AND @EndDate
ORDER BY b.Bill_Date;
GO

-- ================================================
-- COMPLEX QUERIES / REPORTS
-- ================================================

-- Query 44: Get hotel details with room and reservation statistics
SELECT 
    h.Hotel_ID,
    h.Name,
    h.City,
    h.Country,
    h.Rating,
    COUNT(DISTINCT r.Room_ID) AS TotalRooms,
    COUNT(DISTINCT CASE WHEN r.Room_Status = 'Available' THEN r.Room_ID END) AS AvailableRooms,
    COUNT(DISTINCT res.Reservation_ID) AS TotalReservations,
    ISNULL(AVG(r.Room_Price), 0) AS AverageRoomPrice
FROM Hotel h
LEFT JOIN Room r ON h.Hotel_ID = r.Hotel_ID
LEFT JOIN Reservation res ON r.Reservation_ID = res.Reservation_ID
GROUP BY h.Hotel_ID, h.Name, h.City, h.Country, h.Rating
ORDER BY h.Name;
GO

-- Query 45: Get monthly reservation report
SELECT 
    FORMAT(r.CheckIn_Date, 'yyyy-MM') AS Month,
    COUNT(r.Reservation_ID) AS TotalReservations,
    SUM(p.Amount) AS TotalRevenue,
    AVG(p.Amount) AS AverageReservationValue
FROM Reservation r
LEFT JOIN Payment p ON r.Reservation_ID = p.Reservation_ID
WHERE r.Status = 'Confirmed'
  AND r.CheckIn_Date >= DATEADD(MONTH, -6, GETDATE())
GROUP BY FORMAT(r.CheckIn_Date, 'yyyy-MM')
ORDER BY Month DESC;
GO

-- Query 46: Get guest booking history
SELECT 
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    g.Loyalty_ID,
    h.Name AS HotelName,
    rm.Room_Number,
    rm.Room_Type,
    r.CheckIn_Date,
    r.CheckOut_Date,
    DATEDIFF(DAY, r.CheckIn_Date, r.CheckOut_Date) AS Nights,
    p.Amount AS TotalPaid,
    r.Status
FROM Guest g
INNER JOIN Reservation r ON g.Guest_ID = r.Guest_ID
LEFT JOIN Room rm ON r.Reservation_ID = rm.Reservation_ID
LEFT JOIN Hotel h ON rm.Hotel_ID = h.Hotel_ID
LEFT JOIN Payment p ON r.Reservation_ID = p.Reservation_ID
WHERE g.Guest_ID = 1
ORDER BY r.CheckIn_Date DESC;
GO

-- Query 47: Get staff performance (reservations created)
SELECT 
    s.Staff_ID,
    s.First_Name + ' ' + s.Last_Name AS StaffName,
    h.Name AS Hotel,
    COUNT(r.Reservation_ID) AS ReservationsCreated
FROM Staff s
INNER JOIN Hotel h ON s.Hotel_ID = h.Hotel_ID
LEFT JOIN Reservation r ON s.Staff_ID = r.Created_By_Staff_ID
GROUP BY s.Staff_ID, s.First_Name, s.Last_Name, h.Name
HAVING COUNT(r.Reservation_ID) > 0
ORDER BY ReservationsCreated DESC;
GO

-- Query 48: Get occupancy rate by hotel
SELECT 
    h.Name AS Hotel,
    COUNT(r.Room_ID) AS TotalRooms,
    COUNT(CASE WHEN r.Room_Status = 'Occupied' THEN 1 END) AS OccupiedRooms,
    COUNT(CASE WHEN r.Room_Status = 'Available' THEN 1 END) AS AvailableRooms,
    CAST(COUNT(CASE WHEN r.Room_Status = 'Occupied' THEN 1 END) * 100.0 / COUNT(r.Room_ID) AS DECIMAL(5,2)) AS OccupancyRate
FROM Hotel h
LEFT JOIN Room r ON h.Hotel_ID = r.Hotel_ID
GROUP BY h.Hotel_ID, h.Name
ORDER BY OccupancyRate DESC;
GO

-- Query 49: Get revenue by payment method
SELECT 
    p.Method,
    COUNT(p.Payment_ID) AS TransactionCount,
    SUM(p.Amount) AS TotalRevenue,
    AVG(p.Amount) AS AverageTransaction
FROM Payment p
WHERE p.Method != 'Pending'
GROUP BY p.Method
ORDER BY TotalRevenue DESC;
GO

-- Query 50: Get complete reservation details
SELECT 
    r.Reservation_ID,
    g.First_Name + ' ' + g.Last_Name AS GuestName,
    g.Nationality,
    h.Name AS HotelName,
    h.City AS HotelCity,
    rm.Room_Number,
    rm.Room_Type,
    r.CheckIn_Date,
    r.CheckOut_Date,
    DATEDIFF(DAY, r.CheckIn_Date, r.CheckOut_Date) AS Nights,
    r.Num_Guests,
    r.Status,
    p.Amount AS TotalAmount,
    p.Method AS PaymentMethod,
    s.First_Name + ' ' + s.Last_Name AS CreatedByStaff
FROM Reservation r
INNER JOIN Guest g ON r.Guest_ID = g.Guest_ID
LEFT JOIN Room rm ON r.Reservation_ID = rm.Reservation_ID
LEFT JOIN Hotel h ON rm.Hotel_ID = h.Hotel_ID
LEFT JOIN Payment p ON r.Reservation_ID = p.Reservation_ID
LEFT JOIN Staff s ON r.Created_By_Staff_ID = s.Staff_ID
WHERE r.Reservation_ID = 1;
GO

PRINT 'All application queries created successfully!';
PRINT 'Total queries: 50';
GO