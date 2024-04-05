using VideoProjectCore6.DTOs.CommonDto;

#nullable disable
namespace VideoProjectCore6.DTOs
{
    public class EventTypeValues : ValueId
    {
        public string Lang { get; set; }
        public Dictionary<string, string> Captions { get; set; }
    }
}
