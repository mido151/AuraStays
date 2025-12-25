INSERT INTO Users (Username, Email, PasswordHash, Role, HotelId, CreatedAt, IsActive)
values ('Mido_Admin', 'admin@Zewailcity.edu', 
        '123123', 
        'Admin', 1, GETDATE(), 1);

update dbo.Users
set Role = 'Admin'
where Username = 'Mido_Admin'