using Microsoft.EntityFrameworkCore;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.DTOs;

#nullable disable
namespace VideoProjectCore6.Services
{
    public class SysValueRepository : ISysValueRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _IGeneralRepository;

        public SysValueRepository(OraDbContext DbContext, IGeneralRepository iGeneralRepository)
        {
            _DbContext = DbContext;
            _IGeneralRepository = iGeneralRepository;
        }
        /*   public async Task<string> AddForFirst(ArabicValue arabicValue, int docTypeId)
           {

               var shortcut = new SqlParameter("@SHORTCUT", SqlDbType.VarChar);
               shortcut.Direction = ParameterDirection.Output;
               shortcut.Size = 25;
               var type = new SqlParameter("@TYPE", arabicValue.LookupType);
               var lang = new SqlParameter("@LANG", "ar");
               var value = new SqlParameter("@VALUE", arabicValue.Value);
               var docType_id = new SqlParameter("@DOCTYPE_ID", docTypeId);
               await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.AddRecord @TYPE , @LANG, @VALUE,@DOCTYPE_ID, @SHORTCUT out",
                                                                    type, lang, value, docType_id, shortcut);

               return shortcut.Value.ToString();
           }*/

        public async Task<string> AddRecord(int id, string tableName, string fieldName, int parentId, string parentFieldName)
        {
            //var shortcut = new SqlParameter("@SHORTCUT", SqlDbType.VarChar);
            //shortcut.Direction = ParameterDirection.Output;
            //shortcut.Size = 25;
            //var template_id = new SqlParameter("@ID", id);
            //var table_name = new SqlParameter("@TABLENAME", tableName);
            //var field_name = new SqlParameter("@FIELDNAME", fieldName);
            //var parent_id = new SqlParameter("@PARENTID", parentId);
            //var parent_field_name = new SqlParameter("@PARENTFIELDNAME", parentFieldName);

            //await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.NewRecord @TABLENAME,@FIELDNAME,@ID,@PARENTID,@PARENTFIELDNAME, @SHORTCUT out",
            //                                                     table_name, field_name, template_id, parent_id, parent_field_name, shortcut);

            //return shortcut.Value.ToString();
            return "";
        }

        public async Task<string> AddTranslation(TranslationDto translationDto)
        {
            /* var last_id = new SqlParameter("@LASTID", SqlDbType.Int);
             last_id.Direction = ParameterDirection.Output;

             var Shortcut = new SqlParameter("@SHORTCUT", translationDto.Shortcut);
             var lang = new SqlParameter("@LANG", translationDto.Lang);
             var value = new SqlParameter("@VALUE", translationDto.Value);

             await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.AddTranslation  @LANG, @VALUE, @SHORTCUT,@LASTID out",
                                                                  lang, value, Shortcut, last_id);

             return last_id.Value.ToString();*/
            return "";
        }

        public async Task<List<CountryDto>> GetAllCountry(string lang)
        {
            if (lang == "ar")
                return await _DbContext.Countries.AsNoTracking().Where(u => u.UgId != null).Select(u => new CountryDto
                {
                    Id = (int)u.UgId,
                    Name = u.CntCountryAr,
                }).ToListAsync();
            else
                return await _DbContext.Countries.AsNoTracking().Where(u => u.UgId != null).Select(u => new CountryDto
                {
                    Id = (int)u.UgId,
                    Name = u.CntCountryEn,
                }).ToListAsync();
        }

        public async Task<Dictionary<int, string>> GetAllCountryDictionary(string lang)
        {
            if (lang == "ar")
                return await _DbContext.Countries.AsNoTracking().Where(u => u.UgId != null).Select(u => new CountryDto
                {
                    Id = (int)u.UgId,
                    Name = u.CntCountryAr,
                }).ToDictionaryAsync(x => x.Id, x => x.Name);
            else
                return await _DbContext.Countries.AsNoTracking().Where(u => u.UgId != null).Select(u => new CountryDto
                {
                    Id = (int)u.UgId,
                    Name = u.CntCountryEn,
                }).ToDictionaryAsync(x => x.Id, x => x.Name);
        }

        public async Task<int> GetIdByShortcut(string shortcut)
        {
            var lookupValue = await _DbContext.SysLookupValues.AsNoTracking().Where(x => x.Shortcut == shortcut).FirstOrDefaultAsync();
            if (lookupValue != null)
                return lookupValue.Id;
            else return -1;
        }




        /*public async Task<List<IdValueDto>> GetTypeAll(string _lang, string _type)
          {

              var type = new SqlParameter("@TYPE", _type);
              var lang = new SqlParameter("@LANG", _lang);
              var result= await _EngineCoreDBContext.IdValueDto.FromSqlRaw("Exec dbo.get_type_elements @TYPE , @LANG", type, lang)
                                                            .ToListAsync();

              return result;
          }*/

        public async Task<List<EventTypeValues>> GetTypeAll(string lang, string type)
        {
            var query = (from t in _DbContext.SysTranslations
                         join lv in _DbContext.SysLookupValues
                            on t.Shortcut equals lv.Shortcut
                         join lt in _DbContext.SysLookupTypes
                            on lv.LookupTypeId equals lt.Id

                         //where t.Lang.Trim().ToLower() == lang.Trim().ToLower()
                         where lt.Value.Trim().ToLower() == type.Trim().ToLower()

                         select new EventTypeValues
                         {
                             Id = lv.Id,
                             Value = t.Value,
                             Lang = t.Lang,
                         });

            return await query.ToListAsync();
        }
        public async Task<Dictionary<int, string>> GetTypeAllDictionary_(string lang, string type)
        {
            var query = (from t in _DbContext.SysTranslations
                         join lv in _DbContext.SysLookupValues
                            on t.Shortcut equals lv.Shortcut
                         join lt in _DbContext.SysLookupTypes
                            on lv.LookupTypeId equals lt.Id

                         where t.Lang.Trim().ToLower() == lang.Trim().ToLower()
                         where lt.Value.Trim().ToLower() == type.Trim().ToLower()
                         select new ValueId
                         {
                             Id = lv.Id,
                             Value = t.Value,
                         });

            return await query.ToDictionaryAsync(x => x.Id, x => x.Value);
        }
        public async Task<string> GetValueById(int id, string lang)
        {

            Task<string> query = null;
            query = (from lv in _DbContext.SysLookupValues
                     join t in _DbContext.SysTranslations
                     on lv.Shortcut equals t.Shortcut


                     where t.Lang == lang
                     where lv.Id == id
                     select t.Value
                 ).FirstOrDefaultAsync();

            return await query;

        }

        public async Task<string> GetValueByShortcut(string shortcut, string lang)
        {
            /*var query = (from t in _EngineCoreDBContext.SysTranslation
                          where t.Lang == lang
                         where t.Shortcut == shortcut
                         select new { value=t.Value }
                        );
            return await query.FirstOrDefaultAsync().ToString();*/
            SysTranslation translationTable = await _DbContext.SysTranslations.AsNoTracking().Where(x => x.Shortcut == shortcut && x.Lang == lang).FirstOrDefaultAsync();
            if (translationTable != null)
                return translationTable.Value;
            else return "Translation Not Found!";
        }

        public async Task<Dictionary<int, string>> GetTypeAllDictionary(string lang, string type)
        {
            var result = await GetTypeAll(lang, type);
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var r in result)
            {
                dic.Add(r.Id, r.Value);
            }
            return dic;
        }
    }
}
