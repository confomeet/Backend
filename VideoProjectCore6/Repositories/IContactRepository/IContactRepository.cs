using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ContactDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.DTOs.FileDto;


namespace VideoProjectCore6.Repositories.IContactRepository
{
    public interface IContactRepository
    {
        Task<APIResult> Add(ContactDto dto, int addBy, string lang);

        Task<APIResult> Add(ContactIndividualDto dto, int addBy, IFormFile file, string lang);

        Task<APIResult> Update(int id, ContactDto dto, int updateBy, string lang);
        Task<APIResult> Delete(int id, int deletedBy, string lang);
        Task<ListContacts> Contacts(int id, byte? classId, string lang);


        Task<APIResult> ContactById(int id, int currentUserId, string lang);

        //Task<List<ContactDto>> MyContact(int id, string lang)
        Task<List<ContactGetDto>> MyContact(int userId, string lang);
        Task<ListCount> Search(int localUser, string? name, string? email, int pageIndex = 1, int pageSize = 25);
        Task<List<ContactSearchView>> Search(int userId, string toSearch);

        Task<object> MeetingJWT(string meetingId, string hash, string lang);
        Task<APIResult> GenerateDirectContactUrl(int v);

        Task<ListCount> AllMyContacts(SearchFilterDto searchFilterDto, int currentUserId, string lang);

        Task<APIResult> SearchSections(int id, int companyId, string text, string lang);
        Task<APIResult> SearchCompanies(int id, string text, string lang);

        Task<APIResult> SearchDirectManagers(int id, int companyId, string text, string lang);

        Task<APIResult> ContactClasses(string lang);

        Task<APIResult> EditProfilePhoto(int id, FilePostDto filePostDto, bool fromUg, bool updateRoles, string lang);

    }
}
