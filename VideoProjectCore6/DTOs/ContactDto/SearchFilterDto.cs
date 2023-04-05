

#nullable disable
namespace VideoProjectCore6.DTOs.ContactDto
{
    public class SearchFilterDto
    {
        public string text { get; set; } = null;
        public string name { get; set; } = null;
        public string email { get; set; } = null;
        public byte? tabId { get; set; }
        public int pageSize { get; set; } = 25;
        public int pageIndex { get; set; } = 1;
    }
}
