using System.ComponentModel.DataAnnotations;
using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.TabDto
{
    public class TabPostDto
    {
        public int? ParentId { get; set; }
        public Dictionary<string, string> NameShortCut { get; set; }
        public byte TabOrder { get; set; }
        public string Link { get; set; }
        public byte[] Icon { get; set; }
        public string IconString { get; set; }
        public IFormFile IconImage { get; set; }
        public TabPostDto()
        {

        }
        public Tab GetEntity()
        {
            Tab tab = new Tab
            {
                ParentId = ParentId,
                TabOrder = TabOrder,
                Link = Link,
                Icon = Icon,
                IconString = IconString
            };

            return tab;
        }
    }
}
