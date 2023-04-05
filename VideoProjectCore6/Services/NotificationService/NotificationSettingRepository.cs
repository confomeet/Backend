using Microsoft.EntityFrameworkCore;
using System.Transactions;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.INotificationRepository;
#nullable disable
namespace VideoProjectCore6.Services.NotificationService
{
    public class NotificationSettingRepository : INotificationSettingRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IConfiguration _IConfiguration;
        //private readonly ISendNotificationRepository _ISendNotificationRepository;

        public NotificationSettingRepository(OraDbContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, IConfiguration iConfiguration/*, ISendNotificationRepository iSendNotificationRepository*/)
        {
            _DbContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _IConfiguration = iConfiguration;
            // _ISendNotificationRepository = iSendNotificationRepository;

        }

        public async Task<int> AddNotificationTemplateWithDetails(NotificationTemplateWithDetailsPostDto notificationTemplatePostDto, int createdBy, string lang)
        {
            // TODO validation for template and template details, channels id's, check if passed en, Arabic names are distinct.
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            NotificationTemplate notifyTemp = new NotificationTemplate
            {
                NotificationNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE, Constants.NOTIFICATION_TEMPLATE_NAME_SHORTCUT),
                CreatedDate = DateTime.Now,
                CreatedBy = createdBy
            };

            _DbContext.NotificationTemplates.Add(notifyTemp);
            await _DbContext.SaveChangesAsync();
            await _iGeneralRepository.InsertUpdateSingleTranslation(notifyTemp.NotificationNameShortcut, notificationTemplatePostDto.NotificationTemplateShortCutLangValue);

            // add template details.
            foreach (var notificationTemplateDetailDto in notificationTemplatePostDto.NotificationTemplateDetails)
            {
                NotificationTemplateDetail notificationTemplateDetail = new NotificationTemplateDetail
                {
                    NotificationTemplateId = notifyTemp.Id,
                    NotificationChannelId = notificationTemplateDetailDto.NotificationChannelId,
                    ChangeAble = notificationTemplateDetailDto.ChangeAble,
                    TitleShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_TITLE_SHORTCUT),
                    BodyShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_BODY_SHORTCUT),
                    CreatedBy = createdBy
                };

                _DbContext.NotificationTemplateDetails.Add(notificationTemplateDetail);
                await _DbContext.SaveChangesAsync();
                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.TitleShortcut, notificationTemplateDetailDto.TitleShortCutLangValue);
                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.BodyShortcut, notificationTemplateDetailDto.BodyShortCutLangValue);
            }

            scope.Complete();
            return notifyTemp.Id;
        }

        public async Task<List<int>> AddNotificationAction(NotificationActionPostDto notificationActionPostDto, int notificationTemplateId)
        {
            // TODO validate for adding valid details for the notification id and NotificationActionPostDto.
            List<int> res = new List<int>();
            using var transaction = _DbContext.Database.BeginTransaction();

            /* foreach (var actionId in notificationActionPostDto.ActionListId)
             {
                 NotificationAction notificationAction = new NotificationAction
                 {
                     ActionId = actionId,
                     NotificationTemplateId = notificationTemplateId
                 };

                 _DbContext.NotificationAction.Add(notificationAction);
                 await _DbContext.SaveChangesAsync();
                 res.Add(notificationAction.Id);
             }*/
            await transaction.CommitAsync();
            return res;
        }

        public async Task<int> AddNotificationTemplatesToOneAction(NotificationTemplatesActionPostDto notificationTemplatesAction)
        {
            int res = 0;
            using var transaction = _DbContext.Database.BeginTransaction();

            /* List<NotificationAction> newNotifyAction = new List<NotificationAction>();

             var oldNotificationsAction = await _DbContext.NotificationAction.Where(x => x.ActionId == notificationTemplatesAction.ActionId).ToListAsync();
             if (oldNotificationsAction != null || oldNotificationsAction.Count > 0)
             {
                 _DbContext.NotificationAction.RemoveRange(oldNotificationsAction);
                 await _DbContext.SaveChangesAsync();
             }

             foreach (var notificationTemplateId in notificationTemplatesAction.NotificationTemplateIds)
             {
                 if (_DbContext.NotificationTemplates.Any(x => x.Id == notificationTemplateId) && !_DbContext.NotificationAction.Any(x => x.ActionId == notificationTemplatesAction.ActionId && x.NotificationTemplateId == notificationTemplateId) && _DbContext.AdmAction.Any(x => x.Id == notificationTemplatesAction.ActionId))
                 {
                     NotificationAction notifyAct = new NotificationAction
                     {
                         ActionId = notificationTemplatesAction.ActionId,
                         NotificationTemplateId = notificationTemplateId
                     };
                     newNotifyAction.Add(notifyAct);
                 }

             }

             _DbContext.NotificationAction.AddRange(newNotifyAction);*/
            res = await _DbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return res;
        }

        public async Task<bool> DeleteNotificationTemplate(int notificationTemplateId)
        {
            NotificationTemplate notificationTemplate = await _DbContext.NotificationTemplates.Where(x => x.Id == notificationTemplateId).FirstOrDefaultAsync();
            if (notificationTemplate == null)
            {
                return false;
            }

            using var transaction = _DbContext.Database.BeginTransaction();

            /* List<NotificationAction> notificationActions = await _DbContext.NotificationAction.Where(x => x.NotificationTemplateId == notificationTemplateId).ToListAsync();
             if (notificationActions.Count > 0)
             {
                 _DbContext.NotificationAction.RemoveRange(notificationActions);
                 await _DbContext.SaveChangesAsync();
             }*/

            List<NotificationTemplateDetail> notificationTemplateDetails = await _DbContext.NotificationTemplateDetails.Where(x => x.NotificationTemplateId == notificationTemplateId).ToListAsync();
            if (notificationTemplateDetails.Count > 0)
            {
                foreach (var tran in notificationTemplateDetails)
                {
                    await _iGeneralRepository.DeleteTranslation(tran.TitleShortcut);
                    await _iGeneralRepository.DeleteTranslation(tran.BodyShortcut);
                }

                _DbContext.NotificationTemplateDetails.RemoveRange(notificationTemplateDetails);
                await _DbContext.SaveChangesAsync();
            }

            await _iGeneralRepository.DeleteTranslation(notificationTemplate.NotificationNameShortcut);
            _DbContext.NotificationTemplates.Remove(notificationTemplate);
            await _DbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }

        public async Task<NotificationTemplateWithDetailsGetDto> GetAllNotificationDetails(int notifyTemplateId)
        {
            NotificationTemplateWithDetailsGetDto res = new NotificationTemplateWithDetailsGetDto();
            NotificationTemplate NotificationTemplate = await _DbContext.NotificationTemplates.Where(x => x.Id == notifyTemplateId).FirstOrDefaultAsync();
            if (NotificationTemplate == null)
            {
                return res;
            }

            res.NotificationTemplateShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(NotificationTemplate.NotificationNameShortcut);
            res.NotificationTemplateId = notifyTemplateId;

            List<NotificationTemplateDetail> notifyDetails = await _DbContext.NotificationTemplateDetails.Where(x => x.NotificationTemplateId == notifyTemplateId).ToListAsync();

            foreach (var notifyDetail in notifyDetails)
            {
                NotificationTemplateDetailGetDto notifyDetailDto = new NotificationTemplateDetailGetDto
                {
                    BodyShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(notifyDetail.BodyShortcut),
                    TitleShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(notifyDetail.TitleShortcut),
                    ChangeAble = notifyDetail.ChangeAble,
                    NotificationChannelId = notifyDetail.NotificationChannelId
                };

                var channelShortCut = await _DbContext.SysLookupValues.Where(x => x.Id == notifyDetail.NotificationChannelId).Select(x => x.Shortcut).FirstOrDefaultAsync();
                if (channelShortCut != null)
                {
                    notifyDetailDto.ChannelShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(channelShortCut);
                }
                res.NotificationTemplateDetails.Add(notifyDetailDto);
            }
            return res;
        }

        public async Task<List<NotificationTemplateGetDto>> GetAllNotificationTemplates()
        {
            List<NotificationTemplateGetDto> res = new List<NotificationTemplateGetDto>();
            var NotificationTemplates = await _DbContext.NotificationTemplates.ToListAsync();
            if (NotificationTemplates == null)
            {
                return res;
            }

            foreach (var notifyTemp in NotificationTemplates)
            {
                NotificationTemplateGetDto notifyTempDto = new NotificationTemplateGetDto
                {
                    NotificationTemplateId = notifyTemp.Id,
                    NotificationTemplateShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(notifyTemp.NotificationNameShortcut)
                };
                res.Add(notifyTempDto);
            }
            return res;
        }

        public async Task<int> EditNotificationTemplateDetials(NotificationTemplateWithDetailsPostDto notificationTemplateDetails, int templateId)
        {
            NotificationTemplate notifyTemplate = await _DbContext.NotificationTemplates.Where(x => x.Id == templateId).FirstOrDefaultAsync();
            if (notifyTemplate == null)
            {
                return 0;
            }

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _iGeneralRepository.InsertUpdateSingleTranslation(notifyTemplate.NotificationNameShortcut, notificationTemplateDetails.NotificationTemplateShortCutLangValue);

            // replace details.
            var oldDetails = await _DbContext.NotificationTemplateDetails.Where(x => x.NotificationTemplateId == templateId).ToListAsync();
            if (oldDetails != null || oldDetails.Count > 0)
            {
                _DbContext.NotificationTemplateDetails.RemoveRange(oldDetails);
                foreach (var tran in oldDetails)
                {
                    await _iGeneralRepository.DeleteTranslation(tran.TitleShortcut);
                    await _iGeneralRepository.DeleteTranslation(tran.BodyShortcut);
                }
            }
            foreach (var notificationTemplateDetailDto in notificationTemplateDetails.NotificationTemplateDetails)
            {
                NotificationTemplateDetail notificationTemplateDetail = new NotificationTemplateDetail
                {
                    NotificationTemplateId = templateId,
                    NotificationChannelId = notificationTemplateDetailDto.NotificationChannelId,
                    ChangeAble = notificationTemplateDetailDto.ChangeAble,
                    TitleShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_TITLE_SHORTCUT),
                    BodyShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_BODY_SHORTCUT)
                };

                _DbContext.NotificationTemplateDetails.Add(notificationTemplateDetail);
                await _DbContext.SaveChangesAsync();

                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.TitleShortcut, notificationTemplateDetailDto.TitleShortCutLangValue);
                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.BodyShortcut, notificationTemplateDetailDto.BodyShortCutLangValue);
            }
            scope.Complete();
            return templateId;
        }

        public Dictionary<string, string> GetParameterList()
        {
            return Constants.ParameterDic;
        }

        public async Task<List<NotificationLogPostDto>> GetNotificationsForAction(int actionId, int? eventId = null)
        {
            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            List<NotificationTemplateDetailsForOneAction> notificationTemplates = await GetNotificationsDetailsForAction(actionId);
            foreach (var notificationTemplate in notificationTemplates)
            {
                List<NotificationTemplateWithDetailsGetDto> notificationTemplateDetails = notificationTemplate.NotificationTemplateDetails;
                foreach (var notificationTemplateDetail in notificationTemplateDetails)
                {
                    List<NotificationTemplateDetailGetDto> notificationDetails = notificationTemplateDetail.NotificationTemplateDetails;
                    foreach (var notify in notificationDetails)
                    {
                        foreach (KeyValuePair<string, string> entry in notify.TitleShortCutLangValue)
                        {
                            NotificationLogPostDto notification = new NotificationLogPostDto
                            {
                                Lang = entry.Key,
                                NotificationTitle = notify.TitleShortCutLangValue[entry.Key],
                                NotificationBody = notify.BodyShortCutLangValue[entry.Key],
                                NotificationChannelId = notify.NotificationChannelId,
                                EventId = eventId,
                            };
                            res.Add(notification);
                        }
                    }
                }
            }
            return res;
        }

        private async Task<List<NotificationTemplateDetailsForOneAction>> GetNotificationsDetailsForAction(int actionId)
        {

            var finalRes = new List<NotificationTemplateDetailsForOneAction>();


            var res = await _DbContext.NotificationActions.
                  Include(x => x.NotificationTemplate).
                  ThenInclude(x => x.NotificationTemplateDetails).
                  Where(x => x.ActionId == actionId).ToListAsync();


            NotificationTemplateDetailsForOneAction notificationTemplateDetailsForOneAction = new NotificationTemplateDetailsForOneAction();

            foreach (var element in res)
            {

                List<NotificationTemplateWithDetailsGetDto> notificationTemplateWithDetailsGetDtos = new List<NotificationTemplateWithDetailsGetDto>();

                List<NotificationTemplateDetailGetDto> notificationTemplateDetailGetDtos = new List<NotificationTemplateDetailGetDto>();


                notificationTemplateDetailsForOneAction.ActionId = element.ActionId;


                NotificationTemplateWithDetailsGetDto notificationTemplateWithDetailsGetDto = new NotificationTemplateWithDetailsGetDto();


                foreach (var e in element.NotificationTemplate.NotificationTemplateDetails)
                {



                    notificationTemplateWithDetailsGetDto.NotificationTemplateDetails.Add(new NotificationTemplateDetailGetDto
                    {
                        NotificationChannelId = e.NotificationChannelId,
                        BodyShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(e.BodyShortcut),
                        TitleShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(e.TitleShortcut),
                        ChangeAble = e.ChangeAble,
                        ChannelShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(_DbContext.SysLookupValues.Where(x => x.Id == e.NotificationChannelId).Select(z => z.Shortcut).FirstOrDefault())
                    });

                };
                notificationTemplateWithDetailsGetDto.NotificationTemplateId = element.NotificationTemplateId;
                notificationTemplateWithDetailsGetDto.NotificationTemplateShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(element.NotificationTemplate.NotificationNameShortcut);

                notificationTemplateWithDetailsGetDtos.Add(notificationTemplateWithDetailsGetDto);

                notificationTemplateDetailsForOneAction.NotificationTemplateDetails = notificationTemplateWithDetailsGetDtos;

                finalRes.Add(notificationTemplateDetailsForOneAction);
            }

            return finalRes;
        }

        //var res = await _DbContext.NotificationActions.
        //      Include(x => x.NotificationTemplate).
        //      ThenInclude(x => x.NotificationTemplateDetails).
        //      Where(x => x.ActionId == actionId).Select(x => new NotificationTemplateDetailsForOneAction
        //      {
        //          ActionId = x.ActionId,
        //          NotificationTemplateDetails = new List<NotificationTemplateWithDetailsGetDto>
        //          { new NotificationTemplateWithDetailsGetDto
        //               {
        //                  NotificationTemplateId = x.NotificationTemplateId, 
        //                  NotificationTemplateShortCutLangValue = _iGeneralRepository.getTranslationsForShortCut(x.NotificationTemplate.NotificationNameShortcut).Result,
        //                  NotificationTemplateDetails =
        //                    (from y in x.NotificationTemplate.NotificationTemplateDetails select
        //                     new NotificationTemplateDetailGetDto {  NotificationChannelId = y.NotificationChannelId ,
        //                                                             BodyShortCutLangValue =  _iGeneralRepository.getTranslationsForShortCut(y.BodyShortcut).Result,
        //                                                             TitleShortCutLangValue =  _iGeneralRepository.getTranslationsForShortCut(y.TitleShortcut).Result,
        //                                                             ChangeAble = y.ChangeAble,
        //                                                             ChannelShortCutLangValue =  _iGeneralRepository.getTranslationsForShortCut(_DbContext.SysLookupValues.Where(x => x.Id == y.NotificationChannelId).Select(z => z.Shortcut).FirstOrDefault()).Result
        //          }).ToList()

        //      }
        //      }
        //      }
        //      ).ToListAsync();







        public async Task GetAllNotificationDetailsLans(string lang)
        {
            string body = "body: ";
            string title = "address: ";

            if (lang == "ar")
            {
                body = " :النص ";
                title = " :العنوان ";
            }

            var NotificationTemplates = await _DbContext.NotificationTemplates.ToListAsync();

            foreach (var template in NotificationTemplates)
            {

                var tempName = await _iGeneralRepository.GetTranslateByShortCut(lang, template.NotificationNameShortcut);
                File.AppendAllText("Result.txt", template.Id + "   " + tempName + Environment.NewLine);
                List<NotificationTemplateDetail> notifyDetails = await _DbContext.NotificationTemplateDetails.Where(x => x.NotificationTemplateId == template.Id).ToListAsync();
                foreach (var notifyDetail in notifyDetails)
                {
                    var notTitle = await _iGeneralRepository.GetTranslateByShortCut(lang, notifyDetail.TitleShortcut);
                    var notBody = await _iGeneralRepository.GetTranslateByShortCut(lang, notifyDetail.BodyShortcut);

                    var channelShortCut = await _DbContext.SysLookupValues.Where(x => x.Id == notifyDetail.NotificationChannelId).Select(x => x.Shortcut).FirstOrDefaultAsync();
                    var chan = await _iGeneralRepository.GetTranslateByShortCut(lang, channelShortCut);

                    File.AppendAllText("Result.txt", "   " + chan + " :  " + title + notTitle + " , " + body + notBody + Environment.NewLine);
                }

                File.AppendAllText("Result.txt", "  --------------------------------- -------------------  -----------------------------   " + Environment.NewLine);

            }


        }

    }
}
