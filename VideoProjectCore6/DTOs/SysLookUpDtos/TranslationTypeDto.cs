using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationTypeDto
    {
        [Required]
        public string shortcut { get; set; } = string.Empty;
    }
}
