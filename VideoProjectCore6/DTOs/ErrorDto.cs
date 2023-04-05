namespace VideoProjectCore6.DTOs
{
    public class ErrorDto
    {
        public int Id { get; set; }
        public string? Result { get; set; }
        public int? Code { get; set; }
        public List<string>? Message { get; set; }
    }
}
