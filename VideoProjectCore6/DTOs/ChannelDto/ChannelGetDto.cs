using System.ComponentModel.DataAnnotations;
#nullable disable
namespace VideoProjectCore6.DTOs.ChannelDto
{
    public class ChannelGetDto
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string ChannelName { get; set; }
        public string ChannelNameShortcut { get; set; }
    }
}
