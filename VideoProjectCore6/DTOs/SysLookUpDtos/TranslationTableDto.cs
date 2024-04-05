using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationTableDto
    {
        [Required]
        public string shortcut { get; set; } = string.Empty;

        [Required]
        public string lang { get; set; } = string.Empty;

        [Required]
        public string value { get; set; } = string.Empty;
    }
}
