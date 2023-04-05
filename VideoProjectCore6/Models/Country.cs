using System;
using System.Collections.Generic;

#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class Country
    {
        public int CntId { get; set; }
        public string? CntCountryEn { get; set; }
        public string? CntCountryAr { get; set; }
        public string? CntOfficialNameEn { get; set; }
        public string? CntOfficialNameAr { get; set; }
        public string? CntRegionEn { get; set; }
        public string? CntContinentAr { get; set; }
        public string? CntContinentEn { get; set; }
        public string? CntRegionAr { get; set; }
        public string? CntCapitalEn { get; set; }
        public string? CntCapitalAr { get; set; }
        public string? CntIso2 { get; set; }
        public string? CntIso3 { get; set; }
        public string? CntGlobalCode { get; set; }
        public int? CntRegIdFk { get; set; }
        public int? CntConIdFk { get; set; }
        public int? UgId { get; set; }
    }
}
