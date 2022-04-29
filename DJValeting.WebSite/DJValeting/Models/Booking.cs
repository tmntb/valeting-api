using System.ComponentModel.DataAnnotations;

namespace DJValeting.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public Flexibility Flexibility { get; set; }
        [Required]
        public VehicleSize VehicleSize { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Required]
        public int ContactNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        public bool? Approved { get; set; }
        public string Status 
        {
            get
            {
                return Approved.HasValue ? Approved.Value ? "approved" : "pending" : string.Empty;
            }
            set
            {
                Status = value;
            }
        }
    }
}
