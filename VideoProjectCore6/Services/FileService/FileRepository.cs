using System.Net;
using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IFileRepository;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Services.Common;
using VideoProjectCore6.Utilities;
using System.Transactions;
using MimeKit;
using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.Utilities.ErrorHandling.Exceptions;

namespace VideoProjectCore6.Services.FileService
{
    public class FileRepository : IFileRepository
    {
        private readonly IFilesUploaderRepository _iFilesUploaderRepositiory;
        private readonly OraDbContext _context;

        public FileRepository(OraDbContext context, IFilesUploaderRepository iFilesUploaderRepositiory)
        {
            _context = context;
            _iFilesUploaderRepositiory = iFilesUploaderRepositiory;
        }

        public async Task<APIResult> Create(IFormFile file, string lang)
        {
            APIResult result = new APIResult();

            try
            {
                UploadedFileMessage fileUploaderDto;

                try
                {
                    fileUploaderDto = await _iFilesUploaderRepositiory.UploadFile(file, "FilesFolderName", true, true);
                }
                catch
                {
                    return result.FailMe(-1, "Error while uploading a file");
                }

                var sysFile = new Files()
                {
                    FilePath = fileUploaderDto.FileUrl,
                    FileName = file.FileName,
                    FileSize = fileUploaderDto.Size,
                    UserId = null,
                    ContactId = null
                };

                _context.SysFiles.Add(sysFile);

                await _context.SaveChangesAsync();

                return result.SuccessMe(1, Translation.getMessage(lang, "Success"), false, APIResult.RESPONSE_CODE.CREATED, sysFile);

            }

            catch
            {
                return result.FailMe(-1, "Error creating the attachment");
            }
        }

        public async Task<APIResult> Add(Files attachment, string lang)
        {
            APIResult result = new APIResult();


            try
            {
                Files newAttachment = new Files
                {
                    FilePath = attachment.FilePath,
                    FileName = attachment.FileName,

                };
                _context.SysFiles.Add(attachment);
                await _context.SaveChangesAsync();

                return result.SuccessMe(1, Translation.getMessage(lang, "Success"), false, APIResult.RESPONSE_CODE.CREATED, newAttachment);
            }

            catch
            {
                return result.FailMe(-1, "Failed to add");
            }
        }

        public async Task<KeyValuePair<byte[], string>> Download(int fileId, string lang)
        {
            APIResult result = new APIResult();

            //try
            //{
            var file = await _context.SysFiles
                        .Include(a => a.User)
                        .FirstOrDefaultAsync(a => a.Id == fileId);

            if (file == null)
                throw new HttpStatusException("File is not exist", "404", HttpStatusCode.NotFound);

            //var corres = await _chatRepository
            //    .Find(attachment.Message.ChatId, userId);

            //if (corres == null)
            //    throw new HttpErrorException("Chat not found", "404", HttpStatusCode.NotFound);

            var responseMessage = await _iFilesUploaderRepositiory.DownloadFile(file.FilePath);

            return responseMessage;
        }
        
        public async Task<APIResult> Delete(int fileId, string lang)
        {

            APIResult result = new APIResult();

            try
            {

                var file = await _context.SysFiles.FirstOrDefaultAsync(a => a.Id == fileId);

                if (file == null)
                    return result.FailMe(-1, "Attachment not found");

                //var corres = await _chatRepository
                //    .Find(attachment.Email.CorresId.GetValueOrDefault(), userId);

                //if (corres == null)
                //    return result.FailMe(-1, "Correspondence not found");

                _context.SysFiles.Remove(file);
                await _context.SaveChangesAsync();

                await _iFilesUploaderRepositiory.DeleteFile(Path.Combine(_iFilesUploaderRepositiory.GetRootPath(), file.FilePath));
                return result.SuccessMe(1, Translation.getMessage(lang, "Success"), true);

            }
            catch
            {
                return result.FailMe(-1, "Correspondence not found");
            }
        }
        
        public async Task<Files> Update(Files attachment, string lang)
        {
            _context.SysFiles.Update(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }
    }
}