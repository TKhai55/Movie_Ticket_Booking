using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Movie_Ticket_Booking.Models;

namespace Movie_Ticket_Booking.Service
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            Account account = new Account(
                "dnjmjwccs",
                "143146453869787",
                "PuaO6XiRnKESZhH7KSSYSo-WFfY"
            );

            _cloudinary = new Cloudinary(account);
        }

        public string UploadImage(byte[] imageBytes, string publicId)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("image", new MemoryStream(imageBytes)),
                PublicId = publicId
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            return uploadResult.SecureUri.AbsoluteUri;
        }
    }
}
    