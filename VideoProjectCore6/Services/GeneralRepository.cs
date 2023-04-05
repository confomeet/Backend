using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.EventDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.SysLookUpDtos;
using VideoProjectCore6.Hubs;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Utility;
#nullable disable
namespace VideoProjectCore6.Services
{
    public class GeneralRepository : IGeneralRepository
    {
        private readonly OraDbContext _DbContext;

        private readonly IConfiguration _IConfiguration;

        private readonly string _Key = "b14ca5898a4e4133bbce2ea2315a1916";

        public GeneralRepository( OraDbContext _dbContext, IConfiguration iConfiguration)
        {
            _DbContext = _dbContext;
            _IConfiguration = iConfiguration;
        }

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


            var result = allRooms.OrderByDescending(c => c.Id).Where(c => c.EventType != 5 && c.EventType != 6
                && c.MeetingId.ToString().Equals(id.ToString())
                && c.ConfId.Equals(meetingId)).FirstOrDefault();


            if (result == null
                && ((DateTime.Compare(startDate, now) > 0 || ((DateTime.Compare(endDate, now) > 0) 
                && (DateTime.Compare(startDate, now) < 0)))))
            {
                return new EventStatus()
                {
                    Status = 3,
                    Text = lang == "ar" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_COMING][0] 
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_COMING][1],
                };
            }

            else
            {

                if (result != null)
                {
                    if (result.EventType == 1 || result.EventType == 4)
                    {
                        return new EventStatus()
                        {
                            Status = 1,
                            Text = lang == "ar" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED][0]
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_STARTED][1],

                        };
                    }

                    else if (result.EventType == 2)
                    {

                        var eventStart = allRooms.Where(x=> x.EventType == 1 && x.MeetingId == id && x.ConfId.Equals(meetingId)).FirstOrDefault();
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
                Text = lang == "ar" ? Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED][0]
                    : Constants.MeetingStatusValue[Constants.EVENT_TYPE.EVENT_TYPE_CONF_FINISHED][1],
            };
        }


        // Check if partici
        public Boolean CheckParticipantStatus(string email, int id, string meetingId, List<ConfEvent> allRooms, List<ConfUser> allUsers)
        {

            var entryPoint = (from ep in allRooms.OrderByDescending(x => x.Id)
                              join e in allUsers on ep.UserId equals e.Id.ToString()
                              where e.Email == email && ep.MeetingId == id && ep.ConfId.Equals(meetingId)

                              select new
                              {
                                  EventType = ep.EventType
                              }).FirstOrDefault();

            if(entryPoint != null)
            {
                if(entryPoint.EventType == 4)
                {
                    return true;
                }

                else if(entryPoint.EventType == 5 && entryPoint.EventType == 6)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<DTOs.CommonDto.APIResult> GetContactToken(int userId)
        {
            DTOs.CommonDto.APIResult result = new APIResult();

            if (userId < 0)
            {
                return result;
            }

            var tokens = await _DbContext.FcmTokens.Where(u => u.UserId == userId).Select(t => t.Token).ToListAsync();
            return result.SuccessMe(userId, "", true, DTOs.CommonDto.APIResult.RESPONSE_CODE.OK, tokens);
        }

        
        // public async Task<DTOs.CommonDto.APIResult> GetLocalContactId(int userId)
        //    {
        //    APIResult result = new DTOs.CommonDto.APIResult();

        //    int? locatContactId = await _DbContext.Contacts.Where(x => x.ContactId == userId).Select(x => x.ContactId).FirstOrDefaultAsync();

        //    if (locatContactId > 0)
        //    {
        //        return result.SuccessMe(locatContactId);
        //    }
        //    else
        //    {
        //        return result.FailMe(-1, "No matching user");
        //    }

        //    }

        public string generateMeetingToken(User user, string meetingId, bool isAdmin)
        {
            DateTime now = DateTime.Now;

            //int mId = _DbContext.Events.

            UserStruct userInfo = new UserStruct()
            {
                id = Guid.NewGuid().ToString(),
                avatar = "",
                name = user.FullName,
                email = "Not available",
               // groudId = Guid.NewGuid().ToString(), //
            };

            string groupId = Guid.NewGuid().ToString();

            DateTime expirationDate = now.AddDays(1);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = expirationDate - origin;
            double doublec = Math.Floor(diff.TotalSeconds);

            Context contxt = new Context()
            {
                user = userInfo,
                group = groupId
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", _IConfiguration["Meeting:iss"] },
                { "aud", _IConfiguration["Meeting:aud"] },
                { "sub", _IConfiguration["Meeting:sub"] },
                { "room", meetingId},
                { "moderator", isAdmin },
                { "context", contxt },
                { "exp", doublec }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            
            return encoder.Encode(payload, _IConfiguration["Meeting:secret"]);

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

        public  string Base64Decode(string base64EncodedData)
        {

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(Base64UrlDecode(base64EncodedData));

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_Key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static string Base64UrlEncode(string input)
        {

            return input
              .Replace('+', '-')
              .Replace('/', '_');

        }


        private static string Base64UrlDecode(string input)
        {

            return input.Replace('-', '+').Replace('_', '/');
        }


        public void Add<T>(T entity) where T : class
        {
            _DbContext.Add(entity);
        }


        // Take new event type, add it to the existing types.

        public async Task<List<Dictionary<string, string>>> AddNewLookUpType(List<Dictionary<string, string>> eventTypePostDTO, string type)
        {

            List<string> Summary = new List<string>();



                int eventId = await _DbContext.SysLookupTypes.Where(s => s.Value.Equals(type)).Select(s=> s.Id).FirstAsync();

                string shortCut = GenerateShortCut("Sys_Translation", "Shortcut");

                SysLookupValue newValue = new SysLookupValue
                {
                    LookupTypeId = Convert.ToInt32(eventId),
                    Shortcut = shortCut
                };

                _DbContext.Add(newValue);

                //List<Dictionary<string, string>> listOfTranslations = new List<Dictionary<string, string>>();

                //string[] langs = { "en", "ar" };


                //for(int i = 0; i< langs.Length; i++)
                //{
                //    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                //    keyValuePairs.Add(langs[i], eventTypePostDTO.eventType);

                //    listOfTranslations.Add(keyValuePairs);
                //}

                var res = await insertDictionaryTranslationAsync(shortCut, eventTypePostDTO);

            return res;
                //return result.SuccessMe(1, "OK", true, APIResult.RESPONSE_CODE.CREATED, "resr");

        }

        public async Task<DTOs.CommonDto.APIResult> DeleteLookUpType(int id, string type)
        {
            APIResult result = new APIResult();

            try
            {
                int eventId = await _DbContext.SysLookupTypes.Where(s => s.Value.Equals(type)).Select(s => s.Id).FirstAsync();


                var lookUpValue = await _DbContext.SysLookupValues.Where(x => x.Id == id && x.LookupTypeId==eventId).FirstOrDefaultAsync();

                if(lookUpValue != null)
                {
                    int res = await DeleteTranslation(lookUpValue.Shortcut);



                    if (res == 1)
                    {
                        _DbContext.SysLookupValues.Remove(lookUpValue);
                        _DbContext.SaveChanges();
                        return result.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK);

                    }
                    else
                    {

                        return result.FailMe(-1, "Failed to delete");
                    }

                } else
                {
                    return result.FailMe(-1, "Id is not provided or is not exist");
                }
               
            } 
            
            catch
            {
                return result.FailMe(-1, "Failed to delete");
            }

        }

        public async Task<DTOs.CommonDto.APIResult> EditLookUpType(List<Dictionary<string, string>> value, int id, string v)
        {
            APIResult result = new APIResult();


            try
            {

                int eventId = await _DbContext.SysLookupTypes.Where(s => s.Value.Equals(v)).Select(s => s.Id).FirstAsync();


                var lookUpValue = await _DbContext.SysLookupValues.Where(x => x.Id == id && x.LookupTypeId == eventId).FirstOrDefaultAsync();


                await InsertUpdateSingleTranslation(lookUpValue.Shortcut,value);

                return result.SuccessMe(1, "Edit Success", true, APIResult.RESPONSE_CODE.OK);
            } 
            
            catch
            {
                return result.FailMe(-1, "Failed to edit");

            }
        }

        public int AppDevice(string userAgent)
        {
            throw new NotImplementedException();
        }

        public int? CalculateFee(int serviceId)
        {
            throw new NotImplementedException();
        }

        public string ConvertFromHTMLTpPDF(string HTML)
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class
        {
            _DbContext.Remove(entity);
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

        public Task<bool> FindLanguage(string lang)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FindShortCut(string ShortCut)
        {
            throw new NotImplementedException();
        }

        public string GenerateShortCut(string tableName, string columnName)
        {
            string shortCut = tableName + "_" + columnName + this.GetNewValueBySec().ToString();
            return shortCut;
        }

        public string GenerateURL(Dictionary<string, string> DictionaryQueryString, string URL)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AllTranslationDto>> GetAllTranslation(string shortcut)
        {
            Task<List<AllTranslationDto>> query = null;
            query = (from t in _DbContext.SysTranslations
                     where t.Shortcut == shortcut
                     select new AllTranslationDto
                     {
                         shortcut = t.Shortcut,
                         lang = t.Lang,
                         translate = t.Value
                     }).ToListAsync();

            return await query;
        }

        public string GetDecviceInfo(string userAgent)
        {
            throw new NotImplementedException();
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
        public int GetNewValueBySec(string name)
        {
            int sequenceNum = 0;
            var connection = _DbContext.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT nextval('{name}');";
                var intRes = (Int64)cmd.ExecuteScalar();
                sequenceNum = Convert.ToInt32(intRes);
            }
            connection.Close();
            return sequenceNum;
        }

        public int GetNewValueForSMSBySec()
        {
            throw new NotImplementedException();
        }

        public int GetNextSecForPayment()
        {
            throw new NotImplementedException();
        }

        public Task<string> getServiceNameTranslateAsync(string lang, int? PaymentId)
        {
            throw new NotImplementedException();
        }

        public Task<string> getServiceNameTranslateByAppIdAsync(string lang, int? applicationId)
        {
            throw new NotImplementedException();
        }

        public string getTranslateByIdFromLookUpValueId(string lang, int id)
        {
            throw new NotImplementedException();
        }

        //public Task<string> GetTranslateByShortCut(string lang, string shortCut)
        //{
        //    throw new NotImplementedException();
        //}

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

        public Task<List<SysTranslation>> insertListTranslationAsync(List<DTOs.SysLookUpDtos.TranslationDto> TranslationList)
        {
            throw new NotImplementedException();
        }

        public Task<AddNewTransResult> insertTranslation(string shortcut, string value, string currentLanguage)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, List<Dictionary<string, string>> TranslationDictionary)
        {
            List<SysTranslation> ListSysTranslation = new List<SysTranslation>();

            var oldTranslations = await _DbContext.SysTranslations.Where(x => x.Shortcut == shortCut).ToListAsync();
            if (oldTranslations.Count != 0)
            {
                _DbContext.SysTranslations.RemoveRange(oldTranslations);
            }


            await insertDictionaryTranslationAsync(shortCut, TranslationDictionary);

            //foreach (var langVal in TranslationDictionary)
            //{
            //    SysTranslation sysTranslation = new SysTranslation
            //    {
            //        Lang = langVal.Key,
            //        Shortcut = shortCut,
            //        Value = langVal.Value
            //    };

            //    ListSysTranslation.Add(sysTranslation);
            //}

            //await _DbContext.SysTranslations.AddRangeAsync(ListSysTranslation);
            //await _DbContext.SaveChangesAsync();

            return ListSysTranslation;
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

        public async Task<bool> Save()
        {
            return (await _DbContext.SaveChangesAsync()) >= 0;
        }


        public int TypeofSign(int id)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T entity) where T : class
        {
            _DbContext.Update(entity);
        }

        //public async Task<bool> FindLanguage(string lang)
        //{
        //    bool found = true;
        //    var result = await _DBContext.SysLanguage.Where(x => x.Lang == lang).ToListAsync();
        //    if (result.Count == 0)

        //        found = false;

        //    return found;
        //}

        //public async Task<bool> FindShortCut(string ShortCut)
        //{
        //    bool found = true;
        //    var result = await _DBContext.SysTranslation.Where(x => x.Shortcut == ShortCut).ToListAsync();
        //    if (result.Count == 0) found = false;

        //    return found;
        //}


        //public string GenerateShortCut(string tableName, string columnName)
        //{
        //    string shortCut = tableName + "_" + columnName + this.GetNewValueBySec().ToString();
        //    return shortCut;
        //}

        //public Dictionary<string, string> GenerateShortCutForAllPropertiesModel(string model)
        //{
        //    Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
        //    /////////////////////////////////////generate next value
        //    Type targetType = Type.GetType("EngineCoreProject.Models." + model, true);
        //    Activator.CreateInstance(targetType);

        //    PropertyInfo[] Properties = targetType.GetProperties(BindingFlags.Public);
        //    foreach (PropertyInfo Property in Properties)
        //    {
        //        var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
        //        p.Direction = System.Data.ParameterDirection.Output;
        //        _DBContext.Database.ExecuteSqlRaw("set @result = next value for seq", p);
        //        int sequenceNum = (int)p.Value;
        //        string shortcut = model + "_" + Property.Name + "_" + sequenceNum;
        //        MyDictionary.Add(Property.Name, shortcut);

        //    }

        //    return MyDictionary;

        //}

        //public int GetNewValueBySec()
        //{
        //    var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
        //    p.Direction = System.Data.ParameterDirection.Output;
        //    _DBContext.Database.ExecuteSqlRaw("set @result = next value for seq", p);
        //    int sequenceNum = (int)p.Value;
        //    return sequenceNum;
        //}

        //public int GetNewValueForSMSBySec()
        //{
        //    var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
        //    p.Direction = System.Data.ParameterDirection.Output;
        //    _DBContext.Database.ExecuteSqlRaw("set @result = next value for SequenceForMessageRequestId", p);
        //    int sequenceNum = (int)p.Value;
        //    return sequenceNum;
        //}

        //public string getTranslateByIdFromLookUpValueId(string lang, int id)
        //{


        //    var query = from slt in _DBContext.SysLookupValue

        //                join st in _DBContext.SysTranslation on slt.Shortcut equals st.Shortcut


        //                where (slt.Id == id && st.Lang == lang)

        //                select st.Value;
        //    if (query.Count() == 0) return "";
        //    return query.FirstOrDefault().ToString();
        //}

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

        //public async Task<List<SysTranslation>> insertListTranslationAsync(List<TranslationDto> TranslationList)
        //{
        //    List<SysTranslation> ListSysTranslation = new List<SysTranslation>();
        //    foreach (TranslationDto translationDt in TranslationList)
        //    {
        //        SysTranslation sysTranslation = new SysTranslation();
        //        sysTranslation.Lang = translationDt.Lang;
        //        sysTranslation.Shortcut = translationDt.Shortcut;
        //        sysTranslation.Value = translationDt.Value;

        //        SysTranslation translations = await _DBContext.SysTranslation.Where(x => x.Shortcut == translationDt.Shortcut && x.Lang == translationDt.Lang).FirstOrDefaultAsync();


        //        if (translations != null)
        //        {
        //            translations.Value = translationDt.Value;
        //            _DBContext.Update(translations);
        //            await _DBContext.SaveChangesAsync();
        //            ListSysTranslation.Add(translations);

        //        }

        //        else
        //        {

        //            _DBContext.Add(sysTranslation);
        //            await _DBContext.SaveChangesAsync();
        //            ListSysTranslation.Add(sysTranslation);
        //        }


        //    }

        //    return ListSysTranslation;
        //}

        //public async Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, Dictionary<string, string> TranslationDictionary)
        //{
        //    List<SysTranslation> ListSysTranslation = new List<SysTranslation>();

        //    var oldTranslations = await _DBContext.SysTranslation.Where(x => x.Shortcut == shortCut).ToListAsync();
        //    if (oldTranslations.Count != 0)
        //    {
        //        _DBContext.SysTranslation.RemoveRange(oldTranslations);
        //    }

        //    foreach (var langVal in TranslationDictionary)
        //    {
        //        SysTranslation sysTranslation = new SysTranslation
        //        {
        //            Lang = langVal.Key,
        //            Shortcut = shortCut,
        //            Value = langVal.Value
        //        };

        //        ListSysTranslation.Add(sysTranslation);
        //    }

        //    await _DBContext.SysTranslation.AddRangeAsync(ListSysTranslation);
        //    await _DBContext.SaveChangesAsync();

        //    return ListSysTranslation;
        //}

        public async Task<List<Dictionary<string, string>>> insertDictionaryTranslationAsync(string shortCut, List<Dictionary<string, string>> TranslationDictionary)//key lang , value translate value
        {
            List<Dictionary<string, string>> ListSysTranslation = new List<Dictionary<string, string>>();
            //if (!await this.FindShortCut(shortCut))
            //{
            //    Dictionary<string, string> result = new Dictionary<string, string>();
            //    result.Add("error shortcut", shortCut + " : this lashortCutnguage not found ");
            //    ListSysTranslation.Add(result);
            //    return ListSysTranslation;

            //}
            foreach (Dictionary<string, string> entry in TranslationDictionary)//key lang , value translate value
            {

                foreach (KeyValuePair<string, string> subentry in entry)//key lang , value translate value
                {


                    Dictionary<string, string> result = new Dictionary<string, string>();

                    //if (!await this.FindLanguage(subentry.Key))
                    //{
                    //    result.Add("error language", subentry.Key + " :this language not found ");
                    //    ListSysTranslation.Add(result);
                    //    continue;

                    //}

                    SysTranslation sysTranslationEdited = await _DbContext.SysTranslations.Where(x => x.Lang == subentry.Key && x.Shortcut == shortCut).FirstOrDefaultAsync();

                    if (sysTranslationEdited != null)
                    {

                        sysTranslationEdited.Value = subentry.Value;


                        _DbContext.Update(sysTranslationEdited);
                        if (await _DbContext.SaveChangesAsync() > 0)
                            result.Add(sysTranslationEdited.Lang, sysTranslationEdited.Shortcut + " updated it's translation as " + sysTranslationEdited.Value + " in " + sysTranslationEdited.Lang + " language ");
                        else result.Add(sysTranslationEdited.Lang, "couldn't update translation for " + sysTranslationEdited.Shortcut + "  as " + sysTranslationEdited.Value + " in " + sysTranslationEdited.Lang + " language ");


                    }

                    else
                    {

                        SysTranslation sysTranslation = new SysTranslation();
                        sysTranslation.Lang = subentry.Key;
                        sysTranslation.Shortcut = shortCut;
                        sysTranslation.Value = subentry.Value;


                        _DbContext.Add(sysTranslation);
                        if (await _DbContext.SaveChangesAsync() > 0) result.Add(sysTranslation.Lang, sysTranslation.Shortcut + " translated as " + sysTranslation.Value + " in " + sysTranslation.Lang + " language ");
                        else result.Add(sysTranslation.Lang, "couldn't translate " + sysTranslation.Shortcut + "  as " + sysTranslation.Value + " in " + sysTranslation.Lang + " language ");
                    }
                    ListSysTranslation.Add(result);




                }
            }
            return ListSysTranslation;
        }


        //public async Task<AddNewTransResult> insertTrans(int inserttype, string target, string lang, string value)
        //{
        //    AddNewTransResult newTrans = null;

        //    Dictionary<string, string> MyDictionary = new Dictionary<string, string>();

        //    List<string> ModelList = new List<string> {
        //        "Action",
        //        "AdmService",
        //        "AdmServiceStage",
        //        "AdmServiceStageAction",
        //        "Application",
        //        "ApplicationAttachment",
        //        "ApplicationNatoryView",
        //        "ApplicationParty",
        //        "Country",
        //        "DocumentStorage",
        //        "DocumentType",
        //        "EngineCoreDBContext",
        //        "FileConfiguration",
        //        "MasterAttachment",
        //        "NotaryEmail",
        //        "NotaryMessage",
        //        "NotaryPlace",
        //        "Notification",
        //        "NotificationTemplate",
        //        "NotificationTemplateAction",
        //        "NotificationTempleteAction",
        //        "NotificationType",
        //        "PartyTypeTodelete",
        //        "Payment",
        //        "Role",
        //        "Serviceinfo",
        //        "Serviceinfobyid",
        //        "SysLookupType",
        //        "SysTranslation",
        //        "SysLookupValue",
        //        "SysyLanguage",
        //        "TableName",
        //        "Template",
        //        "TemplateParty",
        //        "Term",
        //        "User",
        //        "WordModel",
        //    };
        //    List<string> Summary = new List<string>();


        //    List<string> currentLang = await _DbContext.SysTranslations.Select(x => x.Lang).ToListAsync();
        //    if (!currentLang.Any(str => str.Contains(lang)))
        //    {
        //        Summary.Add(lang + " is not avalible language");
        //        newTrans = new AddNewTransResult(Summary, MyDictionary);
        //        return newTrans;
        //    }


        //    string shortCut = null;
        //    ////////////////////////////////////////////////GET NEXT VALUE///////////////////////////////////
        //    var p = new OracleParameter ("@result", OracleDbType.Int32);
        //    p.Direction = System.Data.ParameterDirection.Output;
        //    _DbContext.Database.ExecuteSqlRaw("set @result = next value for seq", p);
        //    int sequenceNum = (int) p.Value;
        //    ///////////////////////////////////////////////FINISH GET NEXT VALUE /////////////////////////////            
        //    ///////////////////////////////GET SHORTCUT////////////////////////
        //    if (inserttype == 1)
        //    {
        //        try
        //        {
        //            Convert.ToInt32(target);
        //        }
        //        catch (Exception)
        //        {
        //            Summary.Add("ID (" + target + ") isn't integer value, please enter correct ID from sys_lookup_type ");
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }

        //        var result = from SysLookupType in _DbContext.SysLookupTypes
        //                     where SysLookupType.Id == Convert.ToInt32(target)
        //                     select new
        //                     {
        //                         SysLookupType.Value
        //                     };

        //        if (!result.Any())
        //        {
        //            Summary.Add("ID (" + target + ") isn't found in sys_Lookup_Type ");
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }
        //        shortCut = await result.Select(x => x.Value).FirstOrDefaultAsync(); //get shortcut for type

        //        Summary.Add("you get shortCut for type: (" + shortCut + ") from sys_Lookup_Type successfully");

        //    }
        //    else
        //    {

        //        if (!ModelList.Any(str => str.Contains(target)))
        //        {
        //            Summary.Add(target + " is not a table");
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }


        //        shortCut = target;
        //        Summary.Add("you get shortCut for table: (" + target + ")   successfully");
        //    }

        //    shortCut += sequenceNum;

        //    Summary.Add("shortCut (" + shortCut + ") for new value was generated successfully");
        //    MyDictionary.Add("shortcut", shortCut);
        //    ////////////////////////////////END GET SHORTCUT////////////////////////

        //    //////////////////////////////TRY TO INSERT INTO TARTGET TABLE////////////////////////
        //    ///
        //    Object TargetClass = null;
        //    if (inserttype == 1)
        //    {
        //        SysLookupValue newValue = new SysLookupValue
        //        {
        //            LookupTypeId = Convert.ToInt32(target),
        //            Shortcut = shortCut
        //        };

        //        _DbContext.Add(newValue);
        //        if (await _DbContext.SaveChangesAsync() >= 0)
        //        {
        //            Summary.Add("you added row to sys_lookup_value table successfully");


        //            MyDictionary.Add("Id", newValue.Id.ToString());
        //        }

        //    }

        //    else
        //    {
        //        Type TargetType = Type.GetType("EngineCoreProject.Models." + target, true);
        //        TargetClass = (Activator.CreateInstance(TargetType));
        //        string val = shortCut;
        //        PropertyInfo propertyInfo = TargetClass.GetType().GetProperty("Shortcut");
        //        propertyInfo.SetValue(TargetClass, Convert.ChangeType(val, propertyInfo.PropertyType), null);

        //        _DbContext.Add(TargetClass);
        //        if (await _DbContext.SaveChangesAsync() >= 0)
        //        {
        //            Summary.Add("you added row to " + target + " table successfuly");
        //            propertyInfo = TargetClass.GetType().GetProperty("Id");
        //            propertyInfo.GetValue(TargetClass, null);
        //            MyDictionary.Add("Id", propertyInfo.GetValue(TargetClass, null).ToString());

        //        }
        //        else
        //        {
        //            Summary.Add("you couldn't add row to target table successfuly,, error occured");
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }
        //    }


        //    //////////////////////////////FINISH TRY TO INSERT INTO TARTGET TABLE////////////////////////
        //    ////////////////////////////////TRY TO INSERT INTO TRANSLATION TABLE////////////////////////
        //    List<SysTranslation> CurrentLanguage = await _DbContext.SysTranslations.ToListAsync();
        //    foreach (SysTranslation language in CurrentLanguage)
        //    {
        //        SysTranslation newTranslation = new SysTranslation() { Lang = language.Lang, Shortcut = shortCut, Value = value };
        //        _DbContext.SysTranslations.Add(newTranslation);
        //        if (await _DbContext.SaveChangesAsync() >= 0)
        //        {
        //            Summary.Add("you add (" + value + ") to your " + language.Lang + " dictionary successfuly");
        //        }
        //        else
        //        {
        //            Summary.Add("you couldn't add (" + value + ") to your " + language.Lang + " dictionary successfuly, error ocurred");
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }

        //    }

        //    ////////////////////////////////FINISH TRY TO INSERT INTO TRANSLATION TABLE////////////////////////

        //    newTrans = new AddNewTransResult(Summary, MyDictionary);
        //    return newTrans;

        //}

        //public async Task<Dictionary<string, string>> getTranslationsForShortCut(string shortCut)
        //{
        //    Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
        //    List<SysTranslation> translations = await _DBContext.SysTranslation.Where(x => x.Shortcut == shortCut).ToListAsync();

        //    if (translations != null)
        //    {
        //        foreach (SysTranslation tran in translations)
        //        {
        //            MyDictionary.Add(tran.Lang, tran.Value);
        //        }
        //    }
        //    return MyDictionary;
        //}

        //public async Task<AddNewTransResult> insertTranslation(string shortcut, string value, string currentLanguage)
        //{
        //    AddNewTransResult newTrans = null;
        //    Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
        //    List<string> Summary = new List<string>();
        //    MyDictionary.Add("shortcut", shortcut);

        //    SysTranslation tran = await _DBContext.SysTranslation.Where(x => x.Shortcut == shortcut & x.Lang == currentLanguage).FirstOrDefaultAsync();

        //    if (tran != null)
        //    {
        //        tran.Value = value;
        //        _DBContext.SysTranslation.Update(tran);
        //        if (await _DBContext.SaveChangesAsync() >= 0)
        //        {
        //            Summary.Add("you update (" + value + ") to your " + currentLanguage + " dictionary successfuly");
        //            MyDictionary.Add(currentLanguage, value);
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }
        //        else
        //        {
        //            Summary.Add("you couldn't update (" + value + ") to your " + currentLanguage + " dictionary successfuly, error ocurred");
        //            newTrans = new AddNewTransResult(Summary, MyDictionary);
        //            return newTrans;
        //        }

        //    }
        //    else
        //    {








        //        List<SysLanguage> CurrentLanguage = await _DBContext.SysLanguage.ToListAsync();
        //        foreach (SysLanguage language in CurrentLanguage)
        //        {
        //            SysTranslation newTranslation = new SysTranslation() { Lang = language.Lang, Shortcut = shortcut, Value = value };
        //            _DBContext.SysTranslation.Add(newTranslation);
        //            if (await _DBContext.SaveChangesAsync() >= 0)
        //            {
        //                Summary.Add("you add (" + value + ") to your " + language.Lang + " dictionary successfuly");
        //                MyDictionary.Add(language.Lang, value);

        //            }
        //            else
        //            {
        //                Summary.Add("you couldn't add (" + value + ") to your " + language.Lang + " dictionary successfuly, error ocurred");

        //            }

        //        }




        //        newTrans = new AddNewTransResult(Summary, MyDictionary);
        //        return newTrans;
        //    }
        //}



        //public async Task<SysTranslation> updateTranslation(string lang, string value, string shortCut)
        //{
        //    SysTranslation translations = await _DBContext.SysTranslation.Where(x => x.Shortcut == shortCut && x.Lang == lang).FirstOrDefaultAsync();

        //    translations.Value = value;

        //    _DBContext.Update(translations);
        //    if (await _DBContext.SaveChangesAsync() >= 0)
        //    {
        //        return translations;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        //public async Task<int> DeleteTranslation(string shortCut)
        //{
        //    List<SysTranslation> translations = await _DBContext.SysTranslation.Where(x => x.Shortcut == shortCut).ToListAsync();
        //    _DBContext.SysTranslation.RemoveRange(translations);
        //    if (await _DBContext.SaveChangesAsync() >= 0)
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}


        public string SecureHashGenerationHMACSHA256(string StringForHash, string Securekey)
        {
            try
            {
                byte[] stringAfterEncodingByte = null;
                byte[] keydSalt = Encoding.UTF8.GetBytes(Securekey);
                byte[] StringForHashbyte = Encoding.UTF8.GetBytes(StringForHash);
                using (var hmac = new System.Security.Cryptography.HMACSHA256(keydSalt))
                {
                    stringAfterEncodingByte = hmac.ComputeHash(StringForHashbyte);
                }
                string hashString = ToHex(stringAfterEncodingByte, false);
                var plainTextBytes = Encoding.UTF8.GetBytes(hashString);
                string stringAfterEncodingBase64 = Convert.ToBase64String(plainTextBytes);
                return stringAfterEncodingBase64;
            }
            catch
            {
                return "";
            }
        }
        private string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }

        public Task<AddNewTransResult> insertTrans(int inserttype, string target, string lang, string value)
        {
            throw new NotImplementedException();
        }
        //public int? CalculateFee(int serviceId)
        //{
        //    int? result = _DBContext.AdmStage.Where(x => x.ServiceId == serviceId).Sum(x => x.Fee);
        //    return result;
        //}

        //public string GetDecviceInfo(string userAgent)
        //{
        //    DeviceDetectorNET.DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

        //    var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);
        //    var output = result.Success ? result.Match.DeviceType : "Unknown";
        //    var id = _DBContext.SysLookupValue.Where(x => x.Shortcut.Contains(output.ToLower())).Select(y => y.Id).FirstOrDefault();
        //    return output;
        //}

        //public async Task<List<AllTranslationDto>> GetAllTranslation(string shortcut)
        //{
        //    Task<List<AllTranslationDto>> query = null;
        //    query = (from t in _DBContext.SysTranslation
        //             where t.Shortcut == shortcut
        //             select new AllTranslationDto
        //             {
        //                 shortcut = t.Shortcut,
        //                 lang = t.Lang,
        //                 translate = t.Value
        //             }).ToListAsync();

        //    return await query;
        //}

        //public string GenerateURL(Dictionary<string, string> DictionaryQueryString, string URL)
        //{
        //    string URlString = null;
        //    var array = (
        //           from key in DictionaryQueryString.Keys
        //           select string.Format(
        //               "{0}={1}",
        //               key,
        //              DictionaryQueryString[key])
        //           ).ToArray();
        //    URlString = string.Join("&", array);
        //    URlString = URL + "?" + HttpUtility.UrlPathEncode(URlString);
        //    URlString = HttpUtility.UrlPathEncode(URlString);
        //    return URlString;
        //}
        //public string ConvertFromHTMLTpPDF(string HTML)
        //{
        //    string FileName = _pdfFileNaming.TermPdfFileName + GetNewValueBySec() + ".pdf";
        //    var globalSettings = new GlobalSettings
        //    {
        //        ColorMode = ColorMode.Color,
        //        Orientation = Orientation.Portrait,
        //        PaperSize = PaperKind.A4,
        //        Margins = new MarginSettings { Top = 10 },
        //        DocumentTitle = "PDF Report",
        //        Out = @"wwwroot/pdfTerm/" + FileName
        //    };

        //    var objectSettings = new ObjectSettings
        //    {
        //        PagesCount = true,
        //        HtmlContent = HTML,
        //        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
        //    };

        //    var pdf = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = globalSettings,
        //        Objects = { objectSettings }
        //    };

        //    var file = _converter.Convert(pdf);
        //    return "pdfTerm/" + FileName;
        //}



        //public async Task<string> getServiceNameTranslateAsync(string lang, int? PaymentId)
        //{
        //    int? ServiceId = await _DBContext.Payment.Where(x => x.Id == PaymentId).Select(x => x.ServiceId).FirstOrDefaultAsync();
        //    string ServiceShortCut = await _DBContext.AdmService.Where(x => x.Id == ServiceId).Select(x => x.Shortcut).FirstOrDefaultAsync();
        //    string TranslateServiceName = await GetTranslateByShortCut(lang, ServiceShortCut);


        //    return TranslateServiceName;
        //}



        //public async Task<string> getServiceNameTranslateByAppIdAsync(string lang, int? applicationId)
        //{
        //    int? ServiceId = await _DBContext.Application.Where(x => x.Id == applicationId).Select(x => x.ServiceId).FirstOrDefaultAsync();
        //    string ServiceShortCut = await _DBContext.AdmService.Where(x => x.Id == ServiceId).Select(x => x.Shortcut).FirstOrDefaultAsync();
        //    string TranslateServiceName = await GetTranslateByShortCut(lang, ServiceShortCut);


        //    return TranslateServiceName;
        //}

        //public int GetNextSecForPayment()
        //{

        //    var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
        //    p.Direction = System.Data.ParameterDirection.Output;
        //    _DBContext.Database.ExecuteSqlRaw("set @result = next value for SeqForPayment", p);
        //    int sequenceNum = (int)p.Value;
        //    return sequenceNum;
        //}

        //public int AppDevice(string userAgent)
        //{
        //    var query = (
        //        from lv in _DBContext.SysLookupValue.Where(x => x.LookupTypeId == 8077)
        //            //join tr in _DBContext.SysTranslation
        //            //on lv.Shortcut equals tr.Shortcut
        //            //where tr.Lang == "en"

        //        select new
        //        {
        //            id = lv.Id,
        //            channelname = lv.Shortcut
        //        });

        //    DeviceDetectorNET.DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

        //    var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);
        //    var output = result.Success ? result.Match.DeviceType : "Unknown";
        //    var id = query.Where(x => x.channelname.ToLower().Contains(output.ToLower())).Select(z => z.id).FirstOrDefault();

        //    if (id == 0)
        //        return query.Where(x => x.channelname.ToLower().Contains("desktop")).Select(z => z.id).FirstOrDefault(); //11154;
        //    else
        //        return id;
        //}

        //public async Task<ClassDto> getShortCutId(string shortcut)
        //{
        //    Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
        //    List<SysTranslation> translations = await _DBContext.SysTranslation.Where(x => x.Shortcut.ToLower() == shortcut.ToLower()).ToListAsync();
        //    int id = _DBContext.SysLookupValue.Where(x => x.Shortcut.ToLower() == shortcut.ToLower()).Select(y => y.Id).FirstOrDefault();
        //    if (translations != null)
        //    {
        //        foreach (SysTranslation tran in translations)
        //        {
        //            MyDictionary.Add(tran.Lang, tran.Value);
        //        }
        //    }
        //    var result = new ClassDto
        //    {
        //        id = id,
        //        mydic = MyDictionary
        //    };
        //    return result;
        //}

        //public int TypeofSign(int id)
        //{
        //    int result = 0;
        //    int sum = 0;
        //    // applicationParty = new List<ApplicationParty>();
        //    List<ApplicationParty> applicationParty = _DBContext.ApplicationParty.Where(x => x.TransactionId == id && x.SignRequired == true).ToList();
        //    if (applicationParty.Count() > 0)
        //    {
        //        foreach (var par in applicationParty)
        //        {
        //            sum += (int)par.SignType;

        //        }
        //        if (applicationParty.Count() == sum)
        //        {
        //            result = 1;
        //        }
        //        else if (applicationParty.Count() * 2 == sum)
        //        {
        //            result = 2;
        //        }
        //        else
        //        {
        //            result = 3;
        //        }
        //    }
        //    return result;
        //}
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
