using VideoProjectCore6.Models;

namespace VideoProjectCore6.Repositories.IFilesUploader
{
    public interface IFileConfigurationRepository
  {
        Task<FileConfiguration[]> GetFileConfigurations();

        Task<FileConfiguration> GetFileConfigurationById(int id);

         Task<FileConfiguration> GetFileConfigurationByExt(string extention);
      }
}
