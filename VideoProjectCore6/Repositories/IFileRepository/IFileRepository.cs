using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using MimeKit;

namespace VideoProjectCore6.Repositories.IFileRepository;

public interface IFileRepository
{
    Task<APIResult> Create(IFormFile file, string lang);
    Task<KeyValuePair<byte[], string>> Download(int fileId, string lang);
    //Task<APIResult> GetAll(int chatId, int userId, int pageSize, int pageNumber);
    Task<APIResult> Delete(int fileId, string lang);
    Task<Files> Update(Files file, string lang);
}