using VideoProjectCore6.DTOs.FileDto;

namespace VideoProjectCore6.Repositories.IFilesUploader
{
    public interface IFilesUploaderRepository
    {
        Task<UploadedFileMessage> UploadFile(IFormFile file, string targetFolder, string type = "");
        Task<UploadedFileMessage> UploadFile(IFormFile file, string targetFolder, bool targetFromSetting, bool changeFileName, string type = "");
        Task<UploadedFileMessage> UploadFileToTemp(IFormFile file, string type = "");
        CreateFolderMessage CreateFolder(string folderPath);
        bool MoveFile(string source, string target);
        bool RemoveFile(string source);
        bool CopyFile(string source, string target);

        string GetFilePath(string folder, string file);

        int GetFilePageCount(string file);
        string GetFilePath(string file);
        string GetRootPath();
        string GetRootFolder();
        string GetMimeType(string extension);
        public string FromBase64ToImage(string base64image, string target);
        public bool FileExist(string folder, string file);
        public bool FileExist(string file);
        public bool FolderExist(string folder);

        public void StoreQueryObj<T>(T objToStore, string folderName);

        /// <summary>
        ///    Deletes the specified directory and, if indicated, any subdirectories and files
        ///     in the directory.
        /// </summary>
        /// <param name="folder"></param>
        public void DeleteFolder(string folder);
        public List<string> GetFolderFilesNames(string folder);

        public List<string> GetLogsFilesNames(string search);

        public int DeleteTemporaryFiles(DateTime dateToDel);


        Task<KeyValuePair<byte[], string>> DownloadFile(string filePath);

        Task DeleteFile(string filePath);
    }
}
