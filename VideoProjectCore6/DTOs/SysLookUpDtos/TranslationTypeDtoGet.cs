using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationTypeDtoGet
    {
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string Shortcut { get; set; } = string.Empty;

        public string TranslationValue { get; set; } = string.Empty;
    }
    public class AllTranslationTypeDtoGet
    {
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string Shortcut { get; set; } = string.Empty;

        public string TranslationValue { get; set; } = string.Empty;
        public string Lang { get; set; } = string.Empty;
    }
}
