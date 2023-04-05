
#nullable disable
namespace VideoProjectCore6.DTOs.AccountDto
{
    public class QAGetDto
    {
        public int? Id { get; set; }
        public string Question { get; set; }

        public string Answer { get; set; }

        public int? Type_Id { get; set; }

        public int? Party_Id { get; set; }

        public string Remarks { get; set; }

        public string Question_Seq { get; set; }
    }
}
