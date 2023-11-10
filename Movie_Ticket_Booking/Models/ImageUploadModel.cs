namespace Movie_Ticket_Booking.Models
{
    public class ImageUploadModel
    {
        public IFormFile ImageFile { get; set; }
        public string PublicId { get; set; }
    }
}
