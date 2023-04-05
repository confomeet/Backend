using System.Text.Json;

namespace VideoProjectCore6
{
    public class ValidatorException : Exception
    {
        public List<string> AttributeMessages { get; set; }

        public ValidatorException()
        {
            AttributeMessages = new List<string>();
        }
        public string GetMessages()
        {
            return JsonSerializer.Serialize(AttributeMessages);
        }
    }
}
