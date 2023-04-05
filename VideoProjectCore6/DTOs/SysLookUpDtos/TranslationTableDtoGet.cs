using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationTableDtoGet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string shortcut { get; set; }

        [Required]
        public string lang { get; set; }

        [Required]
        public string value { get; set; }
    }
}
