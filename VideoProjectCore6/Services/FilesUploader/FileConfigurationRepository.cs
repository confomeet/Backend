using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IFilesUploader;
#nullable disable
namespace VideoProjectCore6.Services.FilesUploader
{
    public class FileConfigurationRepository : IFileConfigurationRepository
    {


        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _IGeneralRepository;
        public FileConfigurationRepository(OraDbContext DbContext, IGeneralRepository iGeneralRepository)
        {
            _DbContext = DbContext;
            _IGeneralRepository = iGeneralRepository;
        }




        public async Task<FileConfiguration> GetFileConfigurationById(int id)
        {
            id = Convert.ToInt32(id);
            var query = _DbContext.FileConfigurations.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();

        }

        public async Task<FileConfiguration[]> GetFileConfigurations()
        {
            var query = _DbContext.FileConfigurations.ToArrayAsync();
            return await query;
        }


        public async Task<FileConfiguration> GetFileConfigurationByExt(string extention)
        {
            var query = _DbContext.FileConfigurations.Where(a => a.Extension.Contains(extention));
            return await query.FirstOrDefaultAsync();
        }
    }
}
