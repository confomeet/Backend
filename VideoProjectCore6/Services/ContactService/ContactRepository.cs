using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.ContactDto;
using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.DTOs.NotificationDto;
using VideoProjectCore6.DTOs.UserDTO;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IContactRepository;
using VideoProjectCore6.Repositories.IFileRepository;
using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IParticipantRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Utility;
using static VideoProjectCore6.Services.Constants;


#nullable disable
namespace VideoProjectCore6.Services.ContactService
{
    public class ContactRepository : IContactRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IUserRepository _IUserRepository;
        private readonly IConfiguration _IConfiguration;
        private readonly INotificationSettingRepository _INotificationSettingRepository;
        private readonly ISendNotificationRepository _ISendNotificationRepository;
        private readonly IParticipantRepository _IParticipantRepository;
        private readonly IGeneralRepository _generalRepository;
        ValidatorException _exception;

        private readonly IFilesUploaderRepository _IFilesUploaderRepositiory;
        private readonly IFileRepository _IFileRepository;
        public ContactRepository(IFileRepository fileRepository, IFilesUploaderRepository iFilesUploaderRepository, ISendNotificationRepository iSendNotificationRepository, IGeneralRepository generalRepository, IParticipantRepository IParticipantRepository, OraDbContext DBContext, IUserRepository userRepository, IConfiguration iConfiguration, INotificationSettingRepository INotificationSettingRepository)
        {
            _DbContext = DBContext;
            _IUserRepository = userRepository;
            _IConfiguration = iConfiguration;
            _IParticipantRepository = IParticipantRepository;
            _INotificationSettingRepository = INotificationSettingRepository;
            _generalRepository = generalRepository;
            _exception = new ValidatorException();
            _ISendNotificationRepository = iSendNotificationRepository;
            _IFilesUploaderRepositiory = iFilesUploaderRepository;
            _IFileRepository = fileRepository;
        }

        public async Task<APIResult> Add(ContactIndividualDto dto, int addBy, IFormFile file, string lang)
        {
            APIResult result = new APIResult();

            bool isAdmin = _IUserRepository.IsAdmin();

            var currentUserContent = await Contacts(addBy, (byte)Constants.CONTACT_UPPER_TABS.ALL, lang);

            if (currentUserContent.Items.Select(x => x.Email).Contains(dto.Email))
            {
                return result.FailMe(-1, "Contact is already added in your contacts");
            }

            if (dto.Type.Equals(INDIVIDUAL) && dto.SectionId != null && dto.CompanyId == null)
            {
                return result.FailMe(-1, "Contact can not be linked directly to a section");
            }


            var addedFile = await _IFilesUploaderRepositiory.UploadFileToTemp(file);


            if (dto.ContactId == null)
            {
                var userId = await _DbContext.Users.Where(u => u.NormalizedEmail.Equals(dto.Email.ToUpper())).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync();
                if (userId > 0)
                {
                    dto.ContactId = userId;
                } 
                else if(isAdmin)
                {
                    RegisterDTO registerDTO = new RegisterDTO {
                        FullName = dto.DisplayName,
                        Email = dto.Email,
                        PhoneNumber = dto.Mobile,
                        Roles = new List<int?> { Int32.Parse(Constants.USER) }
                    };

                    var res = await _IUserRepository.RegisterAsync(registerDTO, lang, true, true);

                    if(res.Id > 0)
                    {
                        result.Result = "‘User related to this contact is not existed in the system. An email was sent for the user for activation!";
                    }
                }
            }
            Contact contact = new Contact()
            {
                ContactId = dto.ContactId,
                DisplayName = dto.DisplayName,
                UserId = addBy, /*dto.UserId == null || dto.UserId <= 0 ? *//* : dto.UserId*/
                Mobile = dto.Mobile,
                Home = dto.Home,
                Office = dto.Office,
                Email = dto.Email,
                CreatedBy = addBy,
                CreatedDate = DateTime.Now,
                Country = dto.Country,
                Website = dto.Website,
                Address = dto.Address,
                Type = dto.Type,
                JobDesc = dto.JobDesc,
                City = dto.City,
                Specialization = dto.Specialization,
                DirectManageId = dto.DirectManageId,
                CompanyId = dto.CompanyId,
                SectionId = dto.SectionId,
                ImageUrl = addedFile.FileUrl
            };

            try
            {
                await _DbContext.Contacts.AddAsync(contact);
                await _DbContext.SaveChangesAsync();



                if (dto.SectionIds != null && dto.SectionIds.Count() > 0)
                {
                    await AddSectionsToIndividuals(dto.SectionIds, contact.Id);
                }


                if (dto.Individuals != null && dto.Individuals.Count > 0)
                {
                    await AddIndividualsToEvents(dto.Individuals, contact.Id, addBy, lang);

                }


                return result.SuccessMe(contact.Id, "Contact added", true, APIResult.RESPONSE_CODE.CREATED);
            }
            catch
            {
                return result.FailMe(-1, "Error adding contact");
            }
        }

        private async Task<APIResult> AddIndividualsToEvents(List<ContactDto> dtos, int id, int addBy, string lang)
        {
            APIResult result = new APIResult();


            var currentUserContent = await Contacts(addBy, (byte)Constants.CONTACT_UPPER_TABS.ALL, lang);


            List<Contact> contacts = new List<Contact>();

            foreach (ContactDto dto in dtos)
            {


                if (dto.ContactId == null)
                {
                    var userId = await _DbContext.Users.Where(u => u.NormalizedEmail.Equals(dto.Email.ToUpper())).AsNoTracking().Select(x => x.Id).FirstOrDefaultAsync();
                    if (userId > 0)
                    {
                        dto.ContactId = userId;
                    }
                }
                Contact contact = new Contact()
                {
                    ContactId = dto.ContactId,
                    DisplayName = dto.DisplayName,
                    UserId = dto.UserId == null || dto.UserId <= 0 ? addBy : dto.UserId,
                    Mobile = dto.Mobile,
                    Home = dto.Home,
                    Office = dto.Office,
                    Email = dto.Email,
                    CreatedBy = addBy,
                    CreatedDate = DateTime.Now,
                    Country = dto.Country,
                    Website = dto.Website,
                    Address = dto.Address,
                    Type = dto.Type,
                    JobDesc = dto.JobDesc,
                    City = dto.City,
                    Specialization = dto.Specialization,
                    DirectManageId = dto.DirectManageId,
                    CompanyId = dto.CompanyId,
                    SectionId = id
                };


                if (!currentUserContent.Items.Select(x => x.Email).Contains(dto.Email))
                {
                    contacts.Add(contact);
                }

            };



            try
            {
                await _DbContext.Contacts.AddRangeAsync(contacts);
                await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Contact added", true, APIResult.RESPONSE_CODE.CREATED);
            }
            catch
            {
                return result.FailMe(-1, "Error adding contact");
            }
        }

        public async Task<APIResult> Delete(int id, int deletedBy, string lang)
        {
            var result = new APIResult();
            Contact contact = await _DbContext.Contacts.Where(a => a.Id == id).FirstOrDefaultAsync();

            if (deletedBy != contact.CreatedBy && (contact.Type.Equals(COMPANY) || contact.Type.Equals(MANAGER) || contact.Type.Equals(SECTION)))
            {
                return result.FailMe(-1, "You can not delete a shared contact");
            }

            if (contact == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "ContactNotFound"));
            }

            try
            {
                _DbContext.Contacts.Remove(contact);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(id, Translation.getMessage(lang, "sucsessDelete"), true, APIResult.RESPONSE_CODE.OK);
            }
            catch
            {
                return result.FailMe(-1, Translation.getMessage(lang, "faildDelete"));
            }
        }

        // All Individuals Contacts that belong to a company are shared by default with all sections.

        public async Task<ListContacts> Contacts(int id, byte? classId, string lang)
        {
            var userIds = await GetSectionsByIndividuals(id);






            if (classId == (byte)Constants.CONTACT_UPPER_TABS.MYCONTACT)
            {
                var contacts = await _DbContext.Contacts.Where(x => x.UserId == id && x.CompanyId == null && x.Type.Equals(INDIVIDUAL)).Select(dto => new ContactGetDto
                {
                    Id = dto.Id,
                    ContactId = dto.ContactId,
                    DisplayName = dto.DisplayName,
                    UserId = dto.UserId,
                    Mobile = dto.Mobile,
                    Home = dto.Home,
                    Office = dto.Office,
                    Email = dto.Email,
                    Country = dto.Country,
                    Website = dto.Website,
                    Address = dto.Address,
                    Type = dto.Type,
                    JobDesc = dto.JobDesc,
                    City = dto.City,
                    Specialization = dto.Specialization,
                    DirectManageId = dto.DirectManageId,
                    CompanyId = dto.CompanyId,
                    SectionId = dto.SectionId,
                    SectionIds = userIds.Select(x => x.SectionId).ToList(),
                    isActive = UserHandler.ConnectedIds.Contains(dto.ContactId.ToString()),
                    File = dto.UserPhotos.Select(w => new FileGetDto
                    {
                        Id = w.Id,
                        FilePath = w.FilePath,
                        FileName = w.FileName,
                        FileSize = w.FileSize
                    }),

                }).ToListAsync();

                return new ListContacts
                {


                    Items = contacts/*,*/
                    //CompanyId = companyContact != null ? companyContact.CompanyId : null,
                    //isAdmin = _IUserRepository.IsAdmin()
                };

            }

            else if (classId == (byte?)Constants.CONTACT_UPPER_TABS.ORGANIZATION)
            {

                var indiviSections = await _DbContext.IndividualSections.Select(x => x.IndividualId).ToListAsync();

                List<ContactGetDto> firstContact = await _DbContext.Contacts.Where(x =>
                ((x.ContactId == id || x.UserId == id) && (x.CompanyId != null || x.Type.Equals(SECTION)))).Select(dto => new ContactGetDto

                {
                    Id = dto.Id,
                    ContactId = dto.ContactId,
                    DisplayName = dto.DisplayName,
                    UserId = dto.UserId,
                    Mobile = dto.Mobile,
                    Home = dto.Home,
                    Office = dto.Office,
                    Email = dto.Email,
                    Country = dto.Country,
                    Website = dto.Website,
                    Address = dto.Address,
                    Type = dto.Type,
                    City = dto.City,
                    JobDesc = dto.JobDesc,
                    Specialization = dto.Specialization,
                    DirectManageId = dto.DirectManageId,
                    CompanyId = dto.CompanyId,
                    SectionId = dto.SectionId,
                    File = dto.UserPhotos.Select(w => new FileGetDto
                    {
                        Id = w.Id,
                        FilePath = w.FilePath,
                        FileName = w.FileName,
                        FileSize = w.FileSize
                    })

                }).ToListAsync();



                if (firstContact.Count() > 0)

                {

                    var sectionsOfIndivid = await GetSectionsByIndividuals(firstContact.FirstOrDefault().Id);

                    var intArr = sectionsOfIndivid.Select(q => q.SectionId).ToList();

                    if (firstContact.FirstOrDefault().SectionId != null)
                    {
                        intArr.Add(firstContact.FirstOrDefault().SectionId);
                    }


                    var individualSec = await GetIndividualsBySection(intArr);

                    //if(sectionsOfIndivid != null)
                    //{
                    //    individualSec = await GetIndividualsBySection(sectionsOfIndivid.Select(q => q.SectionId).ToList());
                    //}

                    var orgContacts = await _DbContext.Contacts.Where(x =>
                    //x.CompanyId == firstContact.CompanyId  &&
                    //x.IndividualSections.Intersect(sectionsOfIndivid).Any()
                    //&&
                    (x.UserId == id &&
                    (x.Type.Equals(SECTION) || x.Type.Equals(MANAGER) || firstContact.Select(e => e.CompanyId).Contains(x.Id) || x.Type.Equals(COMPANY))) ||
                    (individualSec.Select(e => e.IndividualId).Contains(x.Id))
                    ||
                    (firstContact.Select(q => q.CompanyId).Contains(x.CompanyId)) &&
                    (x.Type.Equals(SECTION)
                    || x.Type.Equals(INDIVIDUAL) || x.Type.Equals(MANAGER))
                    && x.ContactId != id
                    && (x.ShareWith == (byte?) 1 || firstContact.Select(q => q.SectionId).Contains(x.SectionId))).Select(dto => new ContactGetDto
                    {
                        Id = dto.Id,
                        ContactId = dto.ContactId,
                        DisplayName = dto.DisplayName,
                        UserId = dto.UserId,
                        Mobile = dto.Mobile,
                        Home = dto.Home,
                        Office = dto.Office,
                        Email = dto.Email,
                        Country = dto.Country,
                        Website = dto.Website,
                        Address = dto.Address,
                        Type = dto.Type,
                        JobDesc = dto.JobDesc,
                        City = dto.City,
                        Specialization = dto.Specialization,
                        DirectManageId = dto.DirectManageId,
                        CompanyId = dto.CompanyId,
                        SectionId = dto.SectionId,
                        SectionIds = sectionsOfIndivid.Select(x => x.SectionId).ToList(),
                        isActive = UserHandler.ConnectedIds.Contains(dto.ContactId.ToString()),
                        File = dto.UserPhotos.Select(w => new FileGetDto
                        {
                            Id = w.Id,
                            FilePath = w.FilePath,
                            FileName = w.FileName,
                            FileSize = w.FileSize
                        }),

                    }).ToListAsync();

                    return new ListContacts
                    {

                        Items = orgContacts.DistinctBy(e => e.Id).ToList(),
                        CompanyId = firstContact.FirstOrDefault().CompanyId,
                        isAdmin = _IUserRepository.IsAdmin()
                    };
                }
                else
                {
                    var contacts = await _DbContext.Contacts.Where(x => x.UserId == id && x.Type.Equals(COMPANY)).Select(dto => new ContactGetDto
                    {
                        Id = dto.Id,
                        ContactId = dto.ContactId,
                        DisplayName = dto.DisplayName,
                        UserId = dto.UserId,
                        Mobile = dto.Mobile,
                        Home = dto.Home,
                        Office = dto.Office,
                        Email = dto.Email,
                        Country = dto.Country,
                        Website = dto.Website,
                        Address = dto.Address,
                        Type = dto.Type,
                        JobDesc = dto.JobDesc,
                        City = dto.City,
                        Specialization = dto.Specialization,
                        DirectManageId = dto.DirectManageId,
                        CompanyId = dto.CompanyId,
                        SectionId = dto.SectionId,
                        SectionIds = userIds.Select(x => x.SectionId).ToList(),

                        isActive = UserHandler.ConnectedIds.Contains(dto.ContactId.ToString()),
                        File = dto.UserPhotos.Select(w => new FileGetDto
                        {
                            Id = w.Id,
                            FilePath = w.FilePath,
                            FileName = w.FileName,
                            FileSize = w.FileSize
                        }),

                    }).ToListAsync();

                    return new ListContacts
                    {

                        Items = contacts.DistinctBy(e => e.Id).ToList(),
                        CompanyId = null,
                        isAdmin = _IUserRepository.IsAdmin()
                    };
                }

            }
            
            else if (classId == (byte)Constants.CONTACT_UPPER_TABS.ALL || classId == null)
            {

                var contacts = await _DbContext.Contacts.Where(x => x.UserId == id && (x.Type.Equals(INDIVIDUAL) || x.Type.Equals(COMPANY))).Select(dto => new ContactGetDto
                {
                    Id = dto.Id,
                    ContactId = dto.ContactId,
                    DisplayName = dto.DisplayName,
                    UserId = dto.UserId,
                    Mobile = dto.Mobile,
                    Home = dto.Home,
                    Office = dto.Office,
                    Email = dto.Email,
                    Country = dto.Country,
                    Website = dto.Website,
                    Address = dto.Address,
                    Type = dto.Type,
                    JobDesc = dto.JobDesc,
                    City = dto.City,
                    Specialization = dto.Specialization,
                    DirectManageId = dto.DirectManageId,
                    CompanyId = dto.CompanyId,
                    SectionId = dto.SectionId,
                    SectionIds = userIds.Select(x => x.SectionId).ToList(),

                    isActive = UserHandler.ConnectedIds.Contains(dto.ContactId.ToString()),
                    File = dto.UserPhotos.Select(w => new FileGetDto
                    {
                        Id = w.Id,
                        FilePath = w.FilePath,
                        FileName = w.FileName,
                        FileSize = w.FileSize
                    }),

                }).ToListAsync();

                var indiviSections = await _DbContext.IndividualSections.Select(x => x.IndividualId).ToListAsync();

                List<ContactGetDto> firstContact = await _DbContext.Contacts.Where(x => ((x.ContactId == id || x.UserId == id)
                && (x.CompanyId != null || x.Type.Equals(SECTION)))).Select(dto => new ContactGetDto

                {
                    Id = dto.Id,
                    ContactId = dto.ContactId,
                    DisplayName = dto.DisplayName,
                    UserId = dto.UserId,
                    Mobile = dto.Mobile,
                    Home = dto.Home,
                    Office = dto.Office,
                    Email = dto.Email,
                    Country = dto.Country,
                    Website = dto.Website,
                    Address = dto.Address,
                    Type = dto.Type,
                    City = dto.City,
                    JobDesc = dto.JobDesc,
                    Specialization = dto.Specialization,
                    DirectManageId = dto.DirectManageId,
                    CompanyId = dto.CompanyId,
                    SectionId = dto.SectionId,
                    File = dto.UserPhotos.Select(w => new FileGetDto
                    {
                        Id = w.Id,
                        FilePath = w.FilePath,
                        FileName = w.FileName,
                        FileSize = w.FileSize
                    }),

                }).ToListAsync();



                if (firstContact.Count() > 0)
                {

                    var sectionsOfIndivid = await GetSectionsByIndividuals(firstContact.FirstOrDefault().Id);

                    var intArr = sectionsOfIndivid.Select(q => q.SectionId).ToList();

                    if (firstContact.FirstOrDefault().SectionId != null)
                    {
                        intArr.Add(firstContact.FirstOrDefault().SectionId);
                    }


                    var individualSec = await GetIndividualsBySection(intArr);

                    //if(sectionsOfIndivid != null)
                    //{
                    //    individualSec = await GetIndividualsBySection(sectionsOfIndivid.Select(q => q.SectionId).ToList());
                    //}

                    contacts.AddRange(await _DbContext.Contacts.Where(x =>
                    //x.CompanyId == firstContact.CompanyId  &&
                    //x.IndividualSections.Intersect(sectionsOfIndivid).Any()
                    //&&
                    (x.UserId == id &&
                    (x.Type.Equals(SECTION)
                    || x.Type.Equals(INDIVIDUAL) || x.Type.Equals(MANAGER) || firstContact.Select(e => e.CompanyId).Contains(x.Id) || x.Type.Equals(COMPANY))) ||
                    (individualSec.Select(e => e.IndividualId).Contains(x.Id))
                    ||
                    (firstContact.Select(q => q.CompanyId).Contains(x.CompanyId)) &&
                    (x.Type.Equals(SECTION)
                    || x.Type.Equals(INDIVIDUAL) || x.Type.Equals(MANAGER))
                    && x.ContactId != id
                    && (x.ShareWith == (byte?) 1 || firstContact.Select(q => q.SectionId).Contains(x.SectionId))).Select(dto => new ContactGetDto
                    {
                        Id = dto.Id,
                        ContactId = dto.ContactId,
                        DisplayName = dto.DisplayName,
                        UserId = dto.UserId,
                        Mobile = dto.Mobile,
                        Home = dto.Home,
                        Office = dto.Office,
                        Email = dto.Email,
                        Country = dto.Country,
                        Website = dto.Website,
                        Address = dto.Address,
                        Type = dto.Type,
                        JobDesc = dto.JobDesc,
                        City = dto.City,
                        Specialization = dto.Specialization,
                        DirectManageId = dto.DirectManageId,
                        CompanyId = dto.CompanyId,
                        SectionId = dto.SectionId,
                        SectionIds = sectionsOfIndivid.Select(x => x.SectionId).ToList(),
                        isActive = UserHandler.ConnectedIds.Contains(dto.ContactId.ToString()),
                        File = dto.UserPhotos.Select(w => new FileGetDto
                        {
                            Id = w.Id,
                            FilePath = w.FilePath,
                            FileName = w.FileName,
                            FileSize = w.FileSize
                        }),

                    }).ToListAsync());

                    return new ListContacts
                    {

                        Items = contacts.DistinctBy(e => e.Id).ToList(),
                        CompanyId = firstContact.FirstOrDefault().CompanyId,
                        isAdmin = _IUserRepository.IsAdmin()
                    };
                }

                else
                {
                    var companyContact = contacts.Where(x => x.CompanyId != null).FirstOrDefault();

                    return new ListContacts
                    {


                        Items = contacts.DistinctBy(d => d.Id).ToList(),
                        CompanyId = companyContact != null ? companyContact.CompanyId : null,
                        isAdmin = _IUserRepository.IsAdmin()
                    };
                }
            }

            return new ListContacts();
        }

        public async Task<APIResult> Update(int id, ContactDto dto, int updateBy, string lang)
        {
            APIResult result = new APIResult();
            Contact contact = await _DbContext.Contacts.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (contact == null)
            {
                return result.FailMe(-1, Translation.getMessage(lang, "ContactNotFound"));
            }
            contact.ContactId = dto.ContactId;
            contact.Office = dto.Office;
            contact.Email = dto.Email;
            contact.DisplayName = dto.DisplayName;
            contact.Home = dto.Home;
            contact.Mobile = dto.Mobile;
            contact.LastUpdatedBy = updateBy;
            contact.LastUpddatedDate = DateTime.Now;
            contact.JobDesc = dto.JobDesc;
            contact.Address = dto.Address;
            contact.CompanyId = dto.CompanyId;
            contact.DirectManageId = dto.DirectManageId;
            //contact.SectionId = dto.SectionId;
            contact.Specialization = dto.Specialization;
            contact.City = dto.City;
            contact.Website = dto.Website;
            contact.ShareWith = dto.ShareWith == true ? (byte?)1 : (byte?)0;

            try
            {
                _DbContext.Contacts.Update(contact);


                if (dto.SectionIds.Count() > 0)
                {
                    await UpdateSectionsOfIndividuals(dto.SectionIds, contact.Id);
                }


                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(id, "Contact updated successfully");
            }
            catch
            {
                return result.FailMe(-1, "Error Updating contact");
            }
        }

        public async Task<ListCount> Search(int localUserId, string name, string email, int pageIndex, int pageSize)
        {
            //var result = new List<ContactSearchView>();
            //IQueryable<Contact> contacts = Enumerable.Empty<Contact>().AsQueryable();
            //if (!string.IsNullOrWhiteSpace(toSearch))
            //{
            //    contacts = double.TryParse(toSearch, out _) ? _DbContext.Contacts.Where(x =>x.UserId== localUserId && x.Mobile != null && x.Mobile.Contains(toSearch)).AsNoTracking()
            //          : _DbContext.Contacts.Where(x => x.UserId == localUserId && (x.Email != null && x.Email.Contains(toSearch)) || (x.DisplayName != null && x.DisplayName.Contains(toSearch))).AsNoTracking();
            //}
            //result = await contacts.Select(x => new ContactSearchView { Id = x.Id, Email = x.Email, Mobile = x.Mobile, DisplayName = x.DisplayName, ContactId = x.ContactId, UserId = x.UserId }).ToListAsync();
            //return result;


            bool twoArePassed = name != null && email != null;


            APIResult result = new APIResult();

            var query = await _DbContext.Contacts.Where(u => u.UserId == localUserId && (twoArePassed ?

            (u.Email.ToLower().Contains(email.ToLower()) && u.DisplayName.ToLower().Contains(name.ToLower())) :


            email != null ? u.Email.ToLower().Contains(email.ToLower()) : name != null ? u.DisplayName.ToLower().Contains(name.ToLower()) : u.DisplayName.Contains(""))).AsNoTracking().

                        Select(x => new ContactSearchView { Id = x.Id, Email = x.Email, Mobile = x.Mobile, DisplayName = x.DisplayName, ContactId = x.ContactId, UserId = x.UserId }).ToListAsync();

            int total = query.Count();



            return new ListCount
            {
                Count = total,
                Items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            };

        }

        public async Task<ListCount> AllMyContacts(SearchFilterDto searchFilterDto, int currentUserId, string lang)
        {
            APIResult result = new APIResult();


            //try
            //{
            var contacts = await Contacts(currentUserId, searchFilterDto.tabId, lang);
            bool twoArePassed = searchFilterDto.name != null && searchFilterDto.email != null;

            if (contacts.Items != null)
            {
                var filteredContacts = contacts.Items.Where(u => (twoArePassed ?

            (u.Email.ToLower().Contains(searchFilterDto.email.ToLower()) && u.DisplayName.ToLower().Contains(searchFilterDto.name.ToLower())) :


            searchFilterDto.email != null ? u.Email.ToLower().Contains(searchFilterDto.email.ToLower())
            :
            searchFilterDto.name != null ? u.DisplayName.ToLower().Contains(searchFilterDto.name.ToLower())
            :
            searchFilterDto.text != null ? (u.DisplayName.ToLower().Contains(searchFilterDto.text.ToLower()) || u.Email.ToLower().Contains(searchFilterDto.text.ToLower()))

            : true)).Select(x => new ContactSearchView
            {
                Id = x.Id,
                Email = x.Email,
                Home = x.Home,
                Address = x.Address,
                City = x.City,
                Country = x.Country,
                Mobile = x.Mobile,
                DisplayName = x.DisplayName,
                ContactId = x.ContactId,
                UserId = x.UserId,
                Type = x.Type,
                DirectManageId = x.DirectManageId,
                JobDesc = x.JobDesc,
                SectionId = x.SectionId,
                CompanyId = x.CompanyId,
                Office = x.Office,
                Specialization = x.Specialization,
                Website = x.Website,
                ShareWith = x.ShareWith,
                SectionIds = x.SectionIds,
                File = x.File,
            }).ToList();


                return new ListCount
                {
                    Count = filteredContacts.Count(),
                    Items = filteredContacts.Skip((searchFilterDto.pageIndex - 1) * searchFilterDto.pageSize).Take(searchFilterDto.pageSize)
                };

            }

            else
            {
                return new ListCount
                {
                    Count = 0,
                    Items = new List<ContactSearchView>(),
                };
            }

        }

        public async Task<APIResult> ContactById(int id, int currentUserId, string lang)
        {

            APIResult result = new APIResult();


            try
            {
                //var contact = await _DbContext.Contacts.Where(x => x.Id == id).FirstOrDefaultAsync();

                var currentUserContent = await Contacts(currentUserId, (byte)Constants.CONTACT_UPPER_TABS.ALL, lang);

                var contact = currentUserContent.Items.Where(x => x.Id == id).FirstOrDefault();

                if (contact != null)
                {
                    return result.SuccessMe(1, "Success", true, APIResult.RESPONSE_CODE.OK, contact);

                }
                else
                {
                    return result.FailMe(-1, "Not found");
                }
            }

            catch
            {
                return result.FailMe(-1, "Not found");
            }


        }

        public async Task<List<IndividualsSections>> GetSectionsByIndividuals(int? IndividualId)
        {
            var RolesId = await _DbContext.IndividualSections.Include(p => p.Section).Where(x => x.IndividualId == IndividualId).ToListAsync();

            return RolesId;
        }

        public async Task<List<IndividualsSections>> GetIndividualsBySection(List<int?> SectionIds)
        {
            var RolesId = await _DbContext.IndividualSections.Include(p => p.Individual).Where(x => SectionIds.Contains(x.SectionId)).ToListAsync();

            return RolesId;
        }


        // Add sections to individuals
        public async Task<APIResult> AddSectionsToIndividuals(List<int?> sectionIds, int individualsId)
        {
            APIResult result = new APIResult();

            try
            {

                List<IndividualsSections> individualsSections = new List<IndividualsSections>();
                foreach (var ids in sectionIds)
                {
                    IndividualsSections individualsSections1 = new IndividualsSections
                    {
                        IndividualId = individualsId,
                        SectionId = ids
                    };

                    individualsSections.Add(individualsSections1);
                }


                await _DbContext.IndividualSections.AddRangeAsync(individualsSections);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(1, "succeed", true, APIResult.RESPONSE_CODE.OK);

            }
            catch
            {

                return result.FailMe(-1, "Failed");
            }
        }

        public async Task<APIResult> UpdateSectionsOfIndividuals(List<int?> sectionIds, int individualsId)
        {
            APIResult result = new APIResult();

            try
            {

                List<IndividualsSections> existing = await _DbContext.IndividualSections.Where(x => sectionIds.Contains(x.SectionId) && x.IndividualId == individualsId).ToListAsync();

                _DbContext.IndividualSections.RemoveRange(existing);



                List<IndividualsSections> individualsSections = new List<IndividualsSections>();
                foreach (var ids in sectionIds)
                {
                    IndividualsSections individualsSections1 = new IndividualsSections
                    {
                        IndividualId = ids,
                        SectionId = individualsId
                    };

                    individualsSections.Add(individualsSections1);
                }


                await _DbContext.IndividualSections.AddRangeAsync(individualsSections);
                await _DbContext.SaveChangesAsync();
                return result.SuccessMe(1, "succeed", true, APIResult.RESPONSE_CODE.OK);

            }
            catch
            {

                return result.FailMe(-1, "Failed");
            }
        }

        public async Task<APIResult> SearchSections(int id, int companyId, string text, string lang)
        {
            APIResult result = new APIResult();

            try
            {

                var myContacts = await Contacts(id, (byte)Constants.CONTACT_UPPER_TABS.ALL, lang);

                var filteredRes = myContacts.Items.Where(q => (q.CompanyId == companyId)
                && (q.Type != null ? q.Type.Equals(SECTION) : false)
                && (text != null ? q.DisplayName.ToLower().Contains(text.ToLower()) : true)).Select(p => new ValueId
                {
                    Id = p.Id,
                    Value = p.DisplayName

                }).ToList();

                return result.SuccessMe(1, "Sucess", false, APIResult.RESPONSE_CODE.OK, filteredRes);

            }
            catch
            {
                return result.FailMe(-1, "Failed");
            }
            //return new ListCount
            //{
            //    Count = filteredRes.Count(),
            //    Items = filteredRes.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            //};

        }

        public async Task<APIResult> SearchCompanies(int id, string text, string lang)
        {
            //APIResult result = new APIResult();
            //var myContacts = await MyContact(id, lang);
            return await GetContactByType(id, COMPANY, text, lang);
        }





        private async Task<APIResult> GetContactByType(int id, string type, string text, string lang)
        {

            APIResult result = new APIResult();


            try
            {



                var myContacts = await Contacts(id, (byte)Constants.CONTACT_UPPER_TABS.ALL, lang);

                var filteredRes = myContacts.Items.Where(q => (q.Type != null ? q.Type.Equals(type) : false) && (text != null ? q.DisplayName.ToLower().Contains(text.ToLower()) : true)).Select(p => new ValueId
                {
                    Id = p.Id,
                    Value = p.DisplayName
                }).ToList();

                return result.SuccessMe(1, "Sucess", false, APIResult.RESPONSE_CODE.OK, filteredRes);

            }
            catch
            {

                return result.FailMe(-1, "Failed to get contacts");

            }
            //return new ListCount {
            //    Count = filteredRes.Count(),
            //    Items = filteredRes.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            //};
        }


        public async Task<APIResult> SearchDirectManagers(int id, int companyId, string text, string lang)
        {
            APIResult result = new APIResult();

            try
            {

                var myContacts = await Contacts(id, (byte)Constants.CONTACT_UPPER_TABS.ALL, lang);

                var filteredRes = myContacts.Items.Where(q => (q.CompanyId == companyId)
                && (q.Type != null ? q.Type.Equals(MANAGER) : false)
                && (text != null ? q.DisplayName.ToLower().Contains(text.ToLower()) : true)).Select(p => new ValueId
                {
                    Id = p.Id,
                    Value = p.DisplayName

                }).ToList();


                return result.SuccessMe(1, "Sucess", false, APIResult.RESPONSE_CODE.OK, filteredRes);


            }
            catch
            {
                return result.FailMe(-1, "Failed to get contacts");
            }
        }

        public async Task<APIResult> ContactClasses(string lang)
        {
            APIResult result = new();

            try
            {
                List<ValueId> valueIdList = new List<ValueId>();

                for (int i = 0; i < Constants.UpperTabsValues.Count(); i++)
                {
                    ValueId newValueId = new ValueId
                    {
                        Id = i,
                        Value = Constants.UpperTabsValues[(Constants.CONTACT_UPPER_TABS)i][lang],
                    };
                    valueIdList.Add(newValueId);

                }

                result.SuccessMe(1, "Success", false, APIResult.RESPONSE_CODE.OK, valueIdList);

            }
            catch
            {
                result.FailMe(-1, "Failed to get classes");
            }
            return await Task.FromResult(result);
        }

        public async Task<APIResult> EditProfilePhoto(int id, FilePostDto filePostDto, bool fromUg, bool updateRoles, string lang)
        {
            APIResult res = new APIResult();


            try
            {

                var user = await _DbContext.Contacts.Where(u => u.Id == id).Include(e => e.UserPhotos).AsNoTracking().FirstOrDefaultAsync();

                if (user == null)
                {
                    return res.FailMe(-1, "Contact not found");
                }


                if (filePostDto.UserPhoto == null)
                {
                    return res.FailMe(-1, "Please add an image");
                }

                if (filePostDto.UserPhoto != null)
                {
                    if (user.UserPhotos.Count() < 1)
                    {
                        //foreach (var attach in userProfilePostDto.ProfilePhoto)
                        //{
                        var attachCreated = await _IFileRepository.Create(filePostDto.UserPhoto);
                        Files resAttachment = (Files)attachCreated.Result;
                        user.UserPhotos.Add(resAttachment);
                        //}
                    }

                    else
                    {

                        var existingAttach = user.UserPhotos.FirstOrDefault();


                        await _IFileRepository.Delete(existingAttach.Id);

                        user.UserPhotos.Clear();

                        //foreach (var attach in userProfilePostDto.ProfilePhoto)
                        //{
                        var attachCreated = await _IFileRepository.Create(filePostDto.UserPhoto);
                        Files resAttachment = (Files)attachCreated.Result;
                        user.UserPhotos.Add(resAttachment);
                        //}

                    }

                }

                _DbContext.Contacts.Update(user);

                await _DbContext.SaveChangesAsync();

                //scope.Complete();

                return res.SuccessMe(id, Translation.getMessage(lang, "sucsessUpdate"));

            }
            catch
            {

                return res.FailMe(-1, "Could update profile");
            }
        }

        //public async Task<APIResult> AddContactPhoto(int contactId, IFormFile attachment)
        //{
        //    APIResult result = new APIResult();



        //    var currentContact = await _DbContext.Contacts.Where(c => c.Id == contactId).FirstOrDefaultAsync();

        //    if(currentContact == null)
        //    {
        //        return result.FailMe(-1, "Contact not exists");
        //    }


        //    var addedFile = await _IFilesUploaderRepositiory.UploadFileToTemp(file);


        //}

    }
}