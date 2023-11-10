using Microsoft.AspNetCore.Mvc;
using Movie_Ticket_Booking.Service;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class CloudinaryController : Controller
    {
        private readonly CloudinaryService _cloudinaryService;

        public CloudinaryController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload-image")]
        public IActionResult UploadImage([FromForm] ImageUploadModel imageUploadModel)
        {
            if (imageUploadModel.ImageFile == null || imageUploadModel.ImageFile.Length == 0)
            {
                return BadRequest("Hình ảnh không hợp lệ");
            }

            using (var stream = imageUploadModel.ImageFile.OpenReadStream())
            {
                byte[] imageBytes = new byte[stream.Length];
                stream.Read(imageBytes, 0, (int)stream.Length);
                string publicId = imageUploadModel.PublicId; // Đặt tên công khai tùy ý

                string imageUrl = _cloudinaryService.UploadImage(imageBytes, publicId);

                return Ok(new { imageUrl });
            }
        }
    }
}
