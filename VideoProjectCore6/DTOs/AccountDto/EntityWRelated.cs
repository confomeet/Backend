namespace VideoProjectCore6.DTOs.AccountDto
#nullable disable
{
    public class EntityWRelated : EntityDto
    {
        public int? Id { get; set; } // Local user id of target entity
        public List<EntityDto> Entities { get; set; }
    }
}
