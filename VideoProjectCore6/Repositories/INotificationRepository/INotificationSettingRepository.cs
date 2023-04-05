using VideoProjectCore6.DTOs.NotificationDto;

namespace VideoProjectCore6.Repositories.INotificationRepository
{
    public interface INotificationSettingRepository
    {
        Task<int> AddNotificationTemplateWithDetails(NotificationTemplateWithDetailsPostDto notificationTemplatePostDto, int createdBy,string lang);
        Task<List<int>> AddNotificationAction(NotificationActionPostDto notificationActionPostDto, int notificationTemplateId);
        Task<int> AddNotificationTemplatesToOneAction(NotificationTemplatesActionPostDto notificationTemplatesAction);
        Task<bool> DeleteNotificationTemplate(int id);
        Task<List<NotificationTemplateGetDto>> GetAllNotificationTemplates();
        Task<NotificationTemplateWithDetailsGetDto> GetAllNotificationDetails(int notifyTemplateId);
        Task<int> EditNotificationTemplateDetials(NotificationTemplateWithDetailsPostDto notificationTemplateDetails, int templateId);
        Dictionary<string, string> GetParameterList();
        Task <List<NotificationLogPostDto>> GetNotificationsForAction(int actionId,int? eventId=null);
        Task GetAllNotificationDetailsLans(string lang);
    }
}
