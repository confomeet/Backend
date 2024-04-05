using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationValueDtoGet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        [Required]
        public string translationType { get; set; } = string.Empty;
        public string translationValue { get; set; } = string.Empty;
    }
    public class AllTranslationValueDtoGet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        [Required]
        public string translationType { get; set; } = string.Empty;
        public string translationValue { get; set; } = string.Empty;
        public string lang { get; set; } = string.Empty;
    }
}
