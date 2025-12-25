using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<HotelEntity> Hotels { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StaffEntity> Staff { get; set; }
        public DbSet<GuestEntity> Guests { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== MAP ENTITY NAMES TO SQL TABLE NAMES =====
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<HotelEntity>().ToTable("Hotel");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<StaffEntity>().ToTable("Staff");
            modelBuilder.Entity<GuestEntity>().ToTable("Guest");
            modelBuilder.Entity<RoomEntity>().ToTable("Room");
            modelBuilder.Entity<Reservation>().ToTable("Reservation");
            modelBuilder.Entity<Payment>().ToTable("Payment");
            modelBuilder.Entity<Review>().ToTable("Review");

            // Configure relationships

            // User - Hotel relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Hotel)
                .WithMany()
                .HasForeignKey(u => u.HotelId)
                .OnDelete(DeleteBehavior.SetNull);

            // Department - Staff relationship (Head)
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Staff)
                .WithOne(s => s.Department)
                .HasForeignKey(s => s.DeptId)
                .OnDelete(DeleteBehavior.SetNull);

            // Room - Reservation relationship
            modelBuilder.Entity<RoomEntity>()
                .HasOne(r => r.Reservation)
                .WithMany(res => res.Rooms)
                .HasForeignKey(r => r.ReservationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Reservation constraints
            modelBuilder.Entity<Reservation>()
                .ToTable(t => t.HasCheckConstraint("CHK_CheckOut_After_CheckIn", "[CheckOutDate] > [CheckInDate]"));

            // Indexes for performance
            modelBuilder.Entity<RoomEntity>()
                .HasIndex(r => r.HotelId)
                .HasDatabaseName("IX_Room_Hotel");

            modelBuilder.Entity<RoomEntity>()
                .HasIndex(r => r.ReservationId)
                .HasDatabaseName("IX_Room_Reservation");

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.GuestId)
                .HasDatabaseName("IX_Reservation_Guest");

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.CheckInDate, r.CheckOutDate })
                .HasDatabaseName("IX_Reservation_Dates");

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.ReservationId)
                .HasDatabaseName("IX_Payment_Reservation");

            modelBuilder.Entity<StaffEntity>()
                .HasIndex(s => s.HotelId)
                .HasDatabaseName("IX_Staff_Hotel");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_User_Email");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_User_Username");
        }
    }
}