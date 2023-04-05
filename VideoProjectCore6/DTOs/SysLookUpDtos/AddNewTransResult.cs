using System.ComponentModel.DataAnnotations;

namespace VideoProjectCore6.DTOs.SysLookUpDtos
{
    public class AddNewTransResult
    {
        [Required]
        public List<string> messages = new List<string>();
        [Required]
        public Dictionary<string, string> MyDictionary = new Dictionary<string, string>();

        public AddNewTransResult()
        {
        }

        public AddNewTransResult(List<string> _messages, Dictionary<string, string> _MyDictionary)
        {
            messages = _messages;
            MyDictionary = _MyDictionary;

        }


    }
}
