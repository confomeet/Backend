﻿using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class TranslationTypeDtoGet
    {
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string Shortcut { get; set; }

        public string TranslationValue { get; set; }
    }
    public class AllTranslationTypeDtoGet
    {
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string Shortcut { get; set; }

        public string TranslationValue { get; set; }
        public string Lang { get; set; }
    }
}