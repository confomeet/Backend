
#nullable disable
namespace VideoProjectCore6.DTOs.FileDto
{
    public class CreateFolderMessage
    {
        //public string FolderName { get; set; }
        public bool SuccessCreation { get; set; } = false;
        public string Message { get; set; }
        public string FolderPath { get; set; }
    }
}
