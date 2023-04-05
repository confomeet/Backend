namespace VideoProjectCore6.DTOs.CommonDto
#nullable disable
{
    public class UsersTimeRange
    {
        public List<int> UserIds { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public List<UserTimeRange> Flatten()
        {
            var result=new List<UserTimeRange>();
            foreach (var user in UserIds)
            {
                result.Add(new UserTimeRange() { UserId = user, StartDateTime = StartDateTime, EndDateTime = EndDateTime });
            }
            return result;
        }
    }
}
