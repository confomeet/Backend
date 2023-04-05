using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.Utilities.ErrorHandling.Exceptions;
#nullable disable
namespace VideoProjectCore6.Services.FilesUploader
{
    public class FilesUploaderRepository : IFilesUploaderRepository
    {
        private readonly ILogger<FilesUploaderRepository> _logger;
        public static IWebHostEnvironment _webHostEnvironment;
        private readonly IFileConfigurationRepository _IFileConfigurationRepository;
        private readonly IConfiguration _IConfiguration;
        private static readonly IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".gif", "image/gif"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".pdf", "application/pdf"},
                {".png", "image/png"},
                {".pntg", "image/x-macpaint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".zip", "application/x-zip-compressed"}


                };

        public FilesUploaderRepository(IWebHostEnvironment webHostEnvironment, ILogger<FilesUploaderRepository> logger,
                                       IFileConfigurationRepository iFileConfigurationRepository, IConfiguration iConfiguration)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _IFileConfigurationRepository = iFileConfigurationRepository;
            _IConfiguration = iConfiguration;
        }



        public async Task<UploadedFileMessage> UploadFile(IFormFile File, string targetFolder, string type)
        {
            string fileExtension, fileExtensionWithOutDot, newFileName, filepath, fileServer, folder;
            long size;

            folder = _IConfiguration[targetFolder];
            if (folder == null)
            {
                folder = targetFolder;
            }
            if (!FolderExist(folder))
            {
                
                var createRes = CreateFolder(folder);
                if (!createRes.SuccessCreation)
                {
                    return new UploadedFileMessage
                    {
                        Id = -1,
                        FileName = null,
                        SuccessUpload = false,
                        Message = createRes.Message
                    };
                }
            }

            fileServer = _IConfiguration["DocumentStorage"];
            string path = Path.Combine(GetRootPath(), folder);
            UploadedFileMessage message,
             failurMessage = new UploadedFileMessage
             {
                 Id = -1,
                 FileName = null,
                 SuccessUpload = false,
                 Message = "No File Provided!"
             };

            if (File == null || File.Length == 0)
                return failurMessage;
            fileExtension = Path.GetExtension(Path.GetFileName(File.FileName));
            fileExtensionWithOutDot = fileExtension.Substring(1);
            // fileExtension = Path.GetExtension(Path.GetFileName(File.FileName)).Substring(1).ToLower();

            var ext = await _IFileConfigurationRepository.GetFileConfigurationByExt(fileExtensionWithOutDot);
            if (ext == null)
            {
                failurMessage.FileName = File.FileName;
                failurMessage.Message = "Unallowed File Extention";
                return failurMessage;
            }

            if (type != null && type.Length > 2)
                if (ext.Type != type)
                {
                    failurMessage.FileName = File.FileName;
                    failurMessage.Message = "Unallowed File Type";
                    return failurMessage;
                }

            size = File.Length;
            if (size < ext.MinSize || size > ext.MaxSize)
            {
                failurMessage.FileName = File.FileName;
                failurMessage.Message = "Unallowed File Size";
                return failurMessage;
            }

            newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);//Path.GetRandomFileName())
            filepath = Path.Combine(path, newFileName);

            try
            {

                /*using (var memoryStream = new MemoryStream())
                {
                 await File.CopyToAsync(memoryStream);
                  var  result = FileTypeVerifier.CheckStream(memoryStream);*/

                //  using (var stream = new FileStream(filepath, FileMode.Create))
                //  {
                //var result = FileTypeVerifier.FromStream(stream);
                // await File.CopyToAsync(stream);
                //-------------------------------------------------
                //}
                //if (!Directory.Exists()) Directory.CreateDirectory();
                using FileStream fs = System.IO.File.Create(filepath);

                await File.CopyToAsync(fs);
                fs.Flush();
                return message = new UploadedFileMessage
                {
                    Id = 1,
                    FileName = newFileName,
                    SuccessUpload = true,
                    Message = "File uploaded", //+ result.Description+'|'+result.IsVerified+'|'+result.Name,
                    FileUrl = Path.Combine(folder, newFileName),
                    Size = size,
                    MimeType = File.ContentType
                };

                //  }

            }
            catch (Exception e)
            {
                failurMessage.Message = "Error . File not uploaded!";
                return failurMessage;

            }
        }
        public async Task<UploadedFileMessage> UploadFile(IFormFile file, string targetFolder, bool targetFromSetting, bool changeFileName, string type)
        {
            string fileExtension, fileExtensionWithOutDot, newFileName, filepath, folder;
            long size;

            if (targetFromSetting)
            {
                folder = _IConfiguration[targetFolder];
            }
            else
            {
                folder = targetFolder;
                if (!FolderExist(folder))
                {
                    var createRes = CreateFolder(folder);
                    if (!createRes.SuccessCreation)
                    {
                        return new UploadedFileMessage
                        {
                            Id = 0,
                            FileName = null,
                            SuccessUpload = false,
                            Message = createRes.Message
                        };
                    }
                }
            }

            string path = Path.Combine(GetRootPath(), folder);

            UploadedFileMessage result = new UploadedFileMessage
            {
                Id = 0,
                FileName = null,
                SuccessUpload = false,
                Message = "لم يتم تحديد ملف!"
            };

            if (file == null || file.Length == 0)
            {
                return result;
            }

            fileExtension = Path.GetExtension(Path.GetFileName(file.FileName));
            fileExtensionWithOutDot = fileExtension[1..];

            var ext = await _IFileConfigurationRepository.GetFileConfigurationByExt(fileExtensionWithOutDot);
            if (ext == null)
            {
                result.FileName = file.FileName;
                result.Message = "نوع الملف غير مسموح";
                return result;
            }

            if (type != null && type.Length > 2)
            {
                if (ext.Type != type)
                {
                    result.FileName = file.FileName;
                    result.Message = "نوع الملف غير مسموح";
                    return result;
                }
            }

            size = file.Length;
            if (size < ext.MinSize || size > ext.MaxSize)
            {
                result.FileName = file.FileName;
                result.Message = "حجم الملف غير مسموح";
                return result;
            }

            if (changeFileName)
            {
                newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
            }
            else
            {
                newFileName = file.FileName;
            }
            filepath = Path.Combine(path, newFileName);

            try
            {
                using FileStream fs = System.IO.File.Create(filepath);

                await file.CopyToAsync(fs);
                fs.Flush();
                UploadedFileMessage message = new UploadedFileMessage
                {
                    Id = 0,
                    FileName = newFileName,
                    SuccessUpload = true,
                    Message = "تم رفع الملف",
                    FileUrl = Path.Combine(folder, newFileName),
                    Size = size,
                    MimeType = file.ContentType
                };
                return message;
            }
            catch (Exception ex)
            {
                result.Message = "حدث خطأ ما..لم يتم رفع الملف" + "  " + ex.Message;
                if (ex.InnerException != null && ex.InnerException.Message.Length > 0)
                {
                    result.Message += ex.InnerException.Message;
                }
                return result;
            }
        }
        public async Task<UploadedFileMessage> UploadFileToTemp(IFormFile File, string type)
        {
            return await UploadFile(File, "temp_files", type);
        }


        public string GetFilePath(string folder, string file)
        {
            return Path.Combine(GetRootPath(), folder, file);
        }

        public bool FileExist(string folder, string file)
        {

            return File.Exists(Path.Combine(GetRootPath(), folder, file));
        }
        public bool FileExist(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return false;
            }
            return File.Exists(Path.Combine(GetRootPath(), file));
        }
        public string GetFilePath(string file)
        {
            return Path.Combine(GetRootPath(), file);
        }


        public void StoreQueryObj<T>(T objToStore, string folderName)
        {
            try
            {
                if (!FolderExist(folderName))
                {
                    var folder = CreateFolder(folderName);
                    if (!folder.SuccessCreation)
                    {
                        _logger.LogInformation(" error in create the folder" + folderName + " needs admin to fix, error is " + folder.Message);
                        return;
                    }
                }

                var fileName = Path.Combine(GetRootPath(), folderName, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".json");
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                File.WriteAllText(fileName, JsonSerializer.Serialize(objToStore, options));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(" error in saving the file to JSON file, error is  " + ex.Message);
            }
        }

        public int GetFilePageCount(string file)
        {
            int res = 0;
           /* if (FileExist(file))
            {
                PdfReader pdfReader = new PdfReader(Path.Combine(GetRootPath(), file));
                res = pdfReader.NumberOfPages;
            }*/

            return res;
        }




        public string GetRootPath()
        {
            string root = _IConfiguration["DocumentStorage"] == "LOCAL" ? _webHostEnvironment.WebRootPath : _IConfiguration["DocumentStorage"];
            return root;
        }
        public string GetRootFolder()
        {
            return _IConfiguration["DocumentStorage"] == "LOCAL" ? "wwwroot" : "Enotary";
        }

        public string GetMimeType(string filename)
        {
            string extension = Path.GetExtension(filename);
            if (extension != null)
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }
            string mime;
            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }
        public string FromBase64ToImage(string base64image, string target)
        {
            try
            {
                string path = Path.Combine(GetRootPath(), target);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string fileName = String.Concat(Convert.ToString(Guid.NewGuid()), ".png");
                string filepath = Path.Combine(path, fileName);
                byte[] imageBytes = Convert.FromBase64String(base64image);
                File.WriteAllBytes(filepath, imageBytes);
                return filepath = Path.Combine(target, fileName);
            }
            catch
            {
                return "";
            }
        }

        public bool FolderExist(string folder)
        {
            return Directory.Exists(Path.Combine(GetRootPath(), folder));
        }


        public void DeleteFolder(string folder)
        {
            try
            {
                if (FolderExist(folder))
                {
                    Directory.Delete(Path.Combine(GetRootPath(), folder), true);
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        public CreateFolderMessage CreateFolder(string folderPath)
        {

            string fullPath = Path.Combine(GetRootPath(), folderPath);
            CreateFolderMessage CFM = new CreateFolderMessage();
            try
            {
                if (Directory.Exists(fullPath))
                {
                    CFM.Message = "المجلد موجود مسبقا";
                    return CFM;
                }

                DirectoryInfo di = Directory.CreateDirectory(fullPath);
                CFM.Message = "تم إنشاء المجلد";
                CFM.SuccessCreation = true;
                CFM.FolderPath = folderPath;

            }
            catch (Exception)
            {
                CFM.Message = "خطأ في انشاء المجلد";
                CFM.SuccessCreation = false;
            }
            return CFM;
        }


        public bool MoveFile(string source, string destination)
        {
            string sourceFullPath = Path.Combine(GetRootPath(), source);
            string destFullPath = Path.Combine(GetRootPath(), destination);
            try
            {
                File.Move(sourceFullPath, destFullPath);
                return true;
            }
            catch (Exception e)
            {
                var m = e.Message;
                return false;
            }
        }

        public bool RemoveFile(string source)
        {
            string sourceFullPath = Path.Combine(GetRootPath(), source);
            try
            {
                File.Delete(sourceFullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CopyFile(string source, string destination)
        {
            string sourceFullPath = Path.Combine(GetRootPath(), source);
            string destFullPath = Path.Combine(GetRootPath(), destination);
            try
            {
                bool exists = System.IO.Directory.Exists(Path.GetDirectoryName(destFullPath));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(destFullPath));
                }

                File.Copy(sourceFullPath, destFullPath);
                return true;
            }
            catch (Exception e)
            {
                var m = e.Message;
                return false;
            }
        }

        public List<string> GetFolderFilesNames(string folder)
        {
            List<string> filesNames = new List<string>();
            string path = Path.Combine(GetRootPath(), folder);
            string[] filePaths = Directory.GetFiles(path);
            foreach (string s in filePaths)
            {
                filesNames.Add(Path.GetFileName(s));
            }
            return filesNames;

        }

        public List<string> GetLogsFilesNames(string search)
        {
            if (search == null)
            {
                search = "";
            }

            return Directory.GetFiles("Logs").Where(s => s.Contains(search)).Select(x => Path.GetFileName(x)).ToList();
        }


        public int DeleteTemporaryFiles(DateTime dateToDel)
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(GetRootPath(), _IConfiguration["TransactionFolder"]));
            var files = info.GetFiles().Where(p => p.CreationTime < dateToDel && p.CreationTime < DateTime.Now.AddDays(-2)).ToArray();
            int count = 0;
            foreach (var file in files)
            {
                if (RemoveFile(Path.Combine(_IConfiguration["TransactionFolder"], file.Name)))
                {
                    count++;
                }
            }


            var appFiles = _IConfiguration["ApplicationFileFolder"];
            if (!string.IsNullOrEmpty(appFiles))
            {
                info = new DirectoryInfo(Path.Combine(GetRootPath(), appFiles));
                files = info.GetFiles().Where(p => p.CreationTime < dateToDel && p.CreationTime < DateTime.Now.AddDays(-2)).ToArray();
                foreach (var file in files)
                {
                    if (RemoveFile(Path.Combine(appFiles, file.Name)))
                    {
                        count++;
                    }
                }
            }

            var pFiles = _IConfiguration["PartyFileFolder"];
            if (!string.IsNullOrEmpty(pFiles))
            {
                info = new DirectoryInfo(Path.Combine(GetRootPath(), pFiles));
                files = info.GetFiles().Where(p => p.CreationTime < dateToDel && p.CreationTime < DateTime.Now.AddDays(-2)).ToArray();
                foreach (var file in files)
                {
                    if (RemoveFile(Path.Combine(pFiles, file.Name)))
                    {
                        count++;
                    }
                }
            }

            return count;

        }

        public async Task DeleteFile(string filePath)
        {
            //  var reqPath = Path.Combine(_targetFolderPath, filePath);
            if (filePath != null)
            {
                await Task.Run(() => File.Delete(filePath));
            }

        }

        public async Task<KeyValuePair<byte[], string>> DownloadFile(string filePath)
        {

            var fileInfo = new FileInfo(filePath);
            var reqPath = Path.Combine(GetRootPath(), filePath);

            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileInfo.Name + fileInfo.Extension, out contentType))
            {

                throw new HttpStatusException("Content-type is not exist", "404", HttpStatusCode.NotFound);
            }

            return new KeyValuePair<byte[], string>(await File.ReadAllBytesAsync(reqPath), contentType);

        }
    }
}
