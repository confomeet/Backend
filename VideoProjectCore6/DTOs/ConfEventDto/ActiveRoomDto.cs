using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.ConfEventDto
{
public class ActiveRoom
{
    [Required]
    public string MeetingId { get; set; } = null!;

    [Required]
    public string Topic { get; set; } = null!;
}
}