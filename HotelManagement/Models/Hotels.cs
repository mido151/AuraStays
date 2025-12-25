using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    // User Authentication
    public class User
    {
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Customer";

        public int? HotelId { get; set; }

        [ForeignKey("HotelId")]
        public HotelEntity? Hotel { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }

    // Hotel Entity
    public class HotelEntity
    {
        [Key]
        [Column("Hotel_ID")]
        public int HotelId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        [NotMapped]
        public List<string> SlideshowImages => string.IsNullOrEmpty(ImageUrl)
            ? new List<string>
            {
                "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800",
                "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800"
            }
            : new List<string> { ImageUrl, ImageUrl, ImageUrl };

        [NotMapped]
        public List<string> Amenities => new List<string>
        {
            "Free WiFi", "Swimming Pool", "Fitness Center", "Restaurant",
            "Room Service", "Spa", "Parking", "Airport Shuttle"
        };

        public ICollection<RoomEntity> Rooms { get; set; } = new List<RoomEntity>();
        public ICollection<StaffEntity> Staff { get; set; } = new List<StaffEntity>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    // Department Entity
    public class Department
    {
        [Key]
        [Column("Dept_ID")]
        public int DeptId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("Hotel_ID")]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public HotelEntity Hotel { get; set; } = null!;

        [Column("Head_Staff_ID")]
        public int? HeadStaffId { get; set; }

        public ICollection<StaffEntity> Staff { get; set; } = new List<StaffEntity>();
    }

    // Staff Entity
    public class StaffEntity
    {
        [Key]
        [Column("Staff_ID")]
        public int StaffId { get; set; }

        [Required, MaxLength(100)]
        [Column("First_Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("Last_Name")]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DOB { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Salary { get; set; }

        [Column("Dept_ID")]
        public int? DeptId { get; set; }

        [ForeignKey("DeptId")]
        public Department? Department { get; set; }

        [Column("Hotel_ID")]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public HotelEntity Hotel { get; set; } = null!;

        [MaxLength(100)]
        public string? Position { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Guest Entity
    public class GuestEntity
    {
        [Key]
        [Column("Guest_ID")]
        public int GuestId { get; set; }

        [Required, MaxLength(100)]
        [Column("First_Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("Last_Name")]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DOB { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(500)]
        [Column("Address_Street")]
        public string? AddressStreet { get; set; }

        [MaxLength(100)]
        [Column("Address_City")]
        public string? AddressCity { get; set; }

        [MaxLength(100)]
        [Column("Address_Country")]
        public string? AddressCountry { get; set; }

        [MaxLength(50)]
        [Column("Loyalty_ID")]
        public string? LoyaltyId { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    // Room Entity
    public class RoomEntity
    {
        [Key]
        [Column("Room_ID")]
        public int RoomId { get; set; }

        [Required, MaxLength(20)]
        [Column("Room_Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("Room_Type")]
        public string RoomType { get; set; } = string.Empty;

        [Column("Room_Price", TypeName = "decimal(10,2)")]
        public decimal RoomPrice { get; set; }

        [Required, MaxLength(20)]
        [Column("Room_Status")]
        public string RoomStatus { get; set; } = "Available";

        [Column("Hotel_ID")]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public HotelEntity Hotel { get; set; } = null!;

        [Column("Reservation_ID")]
        public int? ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }

        public string? ImageUrl { get; set; }

        public int? Capacity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Size { get; set; }

        [NotMapped]
        public string Name => $"Room {RoomNumber}";

        [NotMapped]
        public string BedType => RoomType.Contains("King") ? "King Bed"
            : RoomType.Contains("Queen") ? "Queen Bed"
            : RoomType.Contains("Suite") ? "King Bed + Sofa Bed"
            : RoomType.Contains("Twin") ? "Twin Beds"
            : "Queen Bed";

        [NotMapped]
        public string View => RoomType.Contains("Suite") || RoomType.Contains("Penthouse") ? "City View"
            : RoomType.Contains("Ocean") || RoomType.Contains("Beach") ? "Ocean View"
            : RoomType.Contains("Mountain") ? "Mountain View"
            : "Garden View";

        [NotMapped]
        public decimal PricePerNight => RoomPrice;
    }

    // Reservation Entity
    public class Reservation
    {
        [Key]
        [Column("Reservation_ID")]
        public int ReservationId { get; set; }

        [Column("Booking_Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [Column("CheckIn_Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Column("CheckOut_Date")]
        public DateTime CheckOutDate { get; set; }

        [Column("Num_Guests")]
        public int NumGuests { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [Column("Guest_ID")]
        public int GuestId { get; set; }

        [ForeignKey("GuestId")]
        public GuestEntity Guest { get; set; } = null!;

        [Column("Created_By_Staff_ID")]
        public int? CreatedByStaffId { get; set; }

        [ForeignKey("CreatedByStaffId")]
        public StaffEntity? CreatedByStaff { get; set; }

        public ICollection<RoomEntity> Rooms { get; set; } = new List<RoomEntity>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    // Payment Entity
    public class Payment
    {
        [Key]
        [Column("Payment_ID")]
        public int PaymentId { get; set; }

        [Column("Payment_Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(50)]
        public string Method { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Currency { get; set; } = "USD";

        [Column("Guest_ID")]
        public int GuestId { get; set; }

        [ForeignKey("GuestId")]
        public GuestEntity Guest { get; set; } = null!;

        [Column("Reservation_ID")]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; } = null!;

        [Column("Staff_ID")]
        public int? StaffId { get; set; }

        [ForeignKey("StaffId")]
        public StaffEntity? Staff { get; set; }
    }

    // Review Entity
    public class Review
    {
        [Key]
        [Column("ReviewId")]
        public int ReviewId { get; set; }

        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public HotelEntity Hotel { get; set; } = null!;

        public int GuestId { get; set; }

        [ForeignKey("GuestId")]
        public GuestEntity Guest { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;
    }
}