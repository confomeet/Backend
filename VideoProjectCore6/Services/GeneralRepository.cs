using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
#nullable disable
namespace VideoProjectCore6.Services
{
    public class GeneralRepository(OraDbContext _dbContext, IConfiguration iConfiguration) : IGeneralRepository
    {
        private readonly OraDbContext _DbContext = _dbContext;

        private readonly IConfiguration _IConfiguration = iConfiguration;

        private readonly string _Key = "b14ca5898a4e4133bbce2ea2315a1916";

        /// <summary>
        /// 1: active
        /// 2: finished
        /// 3: coming

        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="meetingId"></param>
        /// <returns>Status: 1 || 2 || 3 </returns>
        public EventStatus CheckStatus(DateTime startDate, DateTime endDate, int id, string meetingId, string lang, List<ConfEvent> allRooms)
        {
            DateTime now = DateTime.Now;


            var result = allRooms.OrderByDescending(c => c.Id).Where(
                c =>
                    c.EventType != Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE
                    && c.EventType != Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE_LOBBY
                    && c.MeetingId.ToString().Equals(id.ToString())
            ).FirstOrDefault();


            if (result == null
                && ((DateTime.Compare(startDate, now) > 0 || ((DateTime.Compare(endDate, now) > 0) 
                && (DateTime.Compare(startDate, now) < 0)))))
            {
                return new EventStatus()
                {
                    Status = 3,
                    Text = lang == "ru" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_COMING][0] 
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_COMING][1],
                };
            }

            else
            {

                if (result != null)
                {
                    if (result.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED || result.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_JOIN)
                    {
                        return new EventStatus()
                        {
                            Status = 1,
                            Text = lang == "ru" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED][0]
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED][1],

                        };
                    }

                    else if (result.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED)
                    {

                        var eventStart = allRooms.Where(x=> x.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED && x.MeetingId == id.ToString() && x.MeetingId.Equals(meetingId)).FirstOrDefault();
                        TimeSpan ts = new TimeSpan();

                        if (eventStart != null)
                        {
                            ts = result.EventTime - eventStart.EventTime;
                        }
                        

                        EventStatusTime eventStatus = new EventStatusTime()
                        {
                            Status = 2,
                            TimeSpent = (int) Math.Round(ts.TotalMinutes),
                            Text = lang == "ar" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED][0]
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED][1],
                        };

                        return eventStatus;
                    }
                }
            }

            return new EventStatus()
            {
                Status = 2,
                Text = lang == "ru" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED][0]
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED][1],
            };
        }


        // Check if partici
        public Boolean CheckParticipantStatus(string email, int id, string meetingId, List<ConfEvent> allRooms, List<ConfUser> allUsers)
        {

            var entryPoint = (from ep in allRooms.OrderByDescending(x => x.Id)
                              join e in allUsers on ep.UserId equals e.Id.ToString()
                              where e.Email == email && ep.MeetingId == id.ToString()

                              select new
                              {
                                  EventType = ep.EventType
                              }).FirstOrDefault();

            if(entryPoint != null)
            {
                if(entryPoint.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_JOIN)
                {
                    return true;
                }

                else if(entryPoint.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE && entryPoint.EventType == Constants.EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE_LOBBY)
                {
                    return false;
                }
            }

            return false;
        }

        public  string Base64Encode(string plainText)
        {

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_Key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Base64UrlEncode(Convert.ToBase64String(array));


        }

        private static string Base64UrlEncode(string input)
        {

            return input
              .Replace('+', '-')
              .Replace('/', '_');

        }

        public async Task<int> DeleteTranslation(string shortCut)
        {
            List<SysTranslation> translations = await _DbContext.SysTranslations.Where(x => x.Shortcut == shortCut).ToListAsync();
            _DbContext.SysTranslations.RemoveRange(translations);
            if (_DbContext.SaveChanges() >= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public string GenerateShortCut(string tableName, string columnName)
        {
            string shortCut = tableName + "_" + columnName + this.GetNewValueBySec().ToString();
            return shortCut;
        }

        public int GetNewValueBySec()
        {
            int sequenceNum = 0;
            var connection = _DbContext.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT nextval('loginseq');";
                var intRes = (Int64)cmd.ExecuteScalar();
                sequenceNum = Convert.ToInt32(intRes);
            }
            connection.Close();
            return sequenceNum;
        }

        public async Task<Dictionary<string, string>> getTranslationsForShortCut(string shortCut)
        {
            Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
            List<SysTranslation> translations = await _DbContext.SysTranslations.Where(x => x.Shortcut == shortCut).ToListAsync();

            if (translations != null)
            {
                foreach (SysTranslation tran in translations)
                {
                    MyDictionary.Add(tran.Lang, tran.Value);
                }
            }
            return MyDictionary;
        }

        public async Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, Dictionary<string, string> TranslationDictionary)
        {
            List<SysTranslation> ListSysTranslation = new List<SysTranslation>();

            var oldTranslations = await _DbContext.SysTranslations.Where(x => x.Shortcut == shortCut).ToListAsync();
            if (oldTranslations.Count != 0)
            {
                _DbContext.SysTranslations.RemoveRange(oldTranslations);
            }

            foreach (var langVal in TranslationDictionary)
            {
                SysTranslation sysTranslation = new SysTranslation
                {
                    Lang = langVal.Key,
                    Shortcut = shortCut,
                    Value = langVal.Value
                };

                ListSysTranslation.Add(sysTranslation);
            }

            await _DbContext.SysTranslations.AddRangeAsync(ListSysTranslation);
            await _DbContext.SaveChangesAsync();

            return ListSysTranslation;
        }

        public async Task<string> GetTranslateByShortCut(string lang, string shortCut)
        {
            string res = "";
            var query = (from translate in _DbContext.SysTranslations
                         where translate.Shortcut == shortCut && translate.Lang == lang
                         select translate.Value);
            if (query != null)
            {
                res = await query.FirstOrDefaultAsync();
            }

            return res;
        }

        public async Task<Dictionary<int,string>> GetActions(string lang)
        {
            return await (from ac in _DbContext.Actions
                     join t in _DbContext.SysTranslations
                     on ac.Shortcut equals t.Shortcut
                     where t.Lang == lang
                     select new 
                     {
                       Id = ac.Id,
                       Value=t.Value
                     }).ToDictionaryAsync(x=>x.Id,x=>x.Value);


        }
    }
}
