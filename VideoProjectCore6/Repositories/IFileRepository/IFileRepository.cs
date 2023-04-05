using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using MimeKit;

namespace VideoProjectCore6.Repositories.IFileRepository;

public interface IFileRepository
{
    Task<APIResult> Create(IFormFile file);
    Task<KeyValuePair<byte[], string>> Download(int fileId);
    //Task<APIResult> GetAll(int chatId, int userId, int pageSize, int pageNumber);
    Task<APIResult> Delete(int fileId);
    Task<Files> Update(Files file);
}