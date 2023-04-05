using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.TabDto
{

        public class TabGetDto
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Name { get; set; }
            public int TabOrder { get; set; }
            public string Link { get; set; }
            public bool HasAccess { get; set; }
            public byte[] Icon { get; set; }
            public string IconString { get; set; }

            public List<int> rolesId { get; set; }

            public Dictionary<string, string> Captions { get; set; }
          public TabGetDto()
            {

            }

            public static TabGetDto GetDTO(Tab tab)
            {
                TabGetDto dto = new TabGetDto
                {
                    Id = tab.Id,
                    Link = tab.Link,
                    ParentId = tab.ParentId,
                    TabOrder = tab.TabOrder,
                    Icon = tab.Icon,
                    IconString = tab.IconString,
                
                    
                };

                return dto;
            }
        }
    }

