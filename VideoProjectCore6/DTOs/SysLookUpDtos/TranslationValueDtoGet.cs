using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationValueDtoGet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        public string Value { get; set; }
        [Required]
        public string translationType { get; set; }//translationValue
        public string translationValue { get; set; }//translationValue
    }
    public class AllTranslationValueDtoGet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        public string Value { get; set; }
        [Required]
        public string translationType { get; set; }//translationValue
        public string translationValue { get; set; }//translationValue
        public string lang { get; set; }//translationValue
    }
}
