using System.ComponentModel.DataAnnotations;

#nullable disable
namespace VideoProjectCore6.DTOs.FileDto
{
    public class AddDataModel
    {
        public IFormFile File { get; set; }

        [Required]
        public string PartyId { get; set; }
    }
}