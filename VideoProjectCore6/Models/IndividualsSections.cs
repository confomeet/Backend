#nullable disable
namespace VideoProjectCore6.Models
{
    public class IndividualsSections
    {
        public virtual Contact Individual { get; set; }
        public virtual Contact Section { get; set; }

        public int? IndividualId { get; set; }
        
        public int? SectionId { get; set; }
    }
}
