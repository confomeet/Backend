
#nullable disable
namespace VideoProjectCore6.DTOs.FileDto
{
    public class FileGetDto
    {
        public int? Id { get; set; }
        //public int? EmailId { get; set; }


        //public int? CorresId { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }
    }
}
