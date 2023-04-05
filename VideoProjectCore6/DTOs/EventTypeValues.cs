using VideoProjectCore6.DTOs.CommonDto;

#nullable disable
namespace VideoProjectCore6.DTOs
{
    public class EventTypeValues : ValueId
    {
        public int Id { get; set; }
        public string Lang { get; set; }
        public Dictionary<string, string> Captions { get; set; }
    }
}
