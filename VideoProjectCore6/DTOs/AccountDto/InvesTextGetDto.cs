namespace VideoProjectCore6.DTOs.AccountDto
{
    public class InvesTextGetDto
    {
        public List<QAGetDto> Items { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        public bool HasMore { get; set; }

        public int? Count { get; set; }

        public List<QALinksDto> Links { get; set; }
    }
}
