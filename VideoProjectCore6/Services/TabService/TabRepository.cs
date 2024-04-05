using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.TabDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.ITabRepository;
using VideoProjectCore6.Services.Permisssions;
#nullable disable
namespace VideoProjectCore6.Services.TabService
{
    public class TabRepository: ITabRepository
    {
        private readonly OraDbContext _DbContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly UserManager<User> _iUserManager;
        ValidatorException _exception;

        public TabRepository(OraDbContext dbContext, UserManager<User> iUserManager, IGeneralRepository iGeneralRepository)
        {
            _DbContext = dbContext;
            _iGeneralRepository = iGeneralRepository;
            _iUserManager = iUserManager;
            _exception = new ValidatorException();
        }

        public async Task<APIResult> AddTab(TabPermPostDto tabPostDto)
        {
            // TODO: add validations.
            var result=new APIResult();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            Tab newTab = tabPostDto.GetEntity();
            newTab.TabNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.TAB, Constants.TAB_NAME_SHORTCUT);
            await _iGeneralRepository.InsertUpdateSingleTranslation(newTab.TabNameShortcut, tabPostDto.NameShortCut);

            if (tabPostDto.IconImage != null)
            {
                using var ms = new MemoryStream();
                tabPostDto.IconImage.CopyTo(ms);
                newTab.Icon = ms.ToArray();
            }

            try
            {
                await _DbContext.Tabs.AddAsync(newTab);
                await _DbContext.SaveChangesAsync();

                var addedTabId = newTab.Id;

                await insertUpdateRoleClaim(tabPostDto.rolesId, "Tab", addedTabId.ToString());

            }

            catch (Exception)
            {
                
                return result.FailMe(-1, "Failed to update. Role id is not correct or error when adding entity");
            }

            scope.Complete();
            return result.SuccessMe(newTab.Id,"Tab added successfully");
        }


        private async Task<IdentityResult> insertUpdateRoleClaim(int[] rolesId, string claimType ,string claimValue)
        {
            var res = IdentityResult.Success;
            var oldRoleClaims = await _DbContext.RoleClaims.Where(x => x.ClaimValue.Equals(claimValue)).ToListAsync();
            _DbContext.RoleClaims.RemoveRange(oldRoleClaims);
            
            List<RoleClaim> roleActionClaims = new List<RoleClaim>();

            foreach (var x in rolesId)
                {
                    roleActionClaims.Add(new RoleClaim { RoleId = x, ClaimType = claimType, ClaimValue = claimValue });
                }
            
            await _DbContext.RoleClaims.AddRangeAsync(roleActionClaims);
            await _DbContext.SaveChangesAsync();
            
            return res;
        }

        public async Task<APIResult> UpdateTab(TabPermPostDto tabPostDto, int rowId)
        {
            var result = new APIResult();
            try
            {
                using var transaction = _DbContext.Database.BeginTransaction();
                Tab originalTab = await _DbContext.Tabs.Where(a => a.Id == rowId).FirstOrDefaultAsync();

                if (originalTab == null)
                {
                    return result.FailMe(-1,"Tab not found!",false,APIResult.RESPONSE_CODE.PageNotFound);
                }

                if (tabPostDto.IconImage != null)
                {
                    using var ms = new MemoryStream();
                    tabPostDto.IconImage.CopyTo(ms);
                    originalTab.Icon = ms.ToArray();
                }

                originalTab.ParentId = tabPostDto.ParentId;
                originalTab.TabOrder = tabPostDto.TabOrder;
                originalTab.Link = tabPostDto.Link;
                originalTab.IconString = tabPostDto.IconString;

                _DbContext.Tabs.Update(originalTab);
                _DbContext.SaveChanges();


                await insertUpdateRoleClaim(tabPostDto.rolesId, "Tab", originalTab.Id.ToString());

                await _iGeneralRepository.InsertUpdateSingleTranslation(originalTab.TabNameShortcut, tabPostDto.NameShortCut);
                transaction.Commit();
                return result.SuccessMe(originalTab.Id,"Tab updated successfully");
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += " inner error is " + ex.InnerException.Message;
                }
                return result.FailMe(-1, msg);
            }
        }

        public async Task<int> UpdateIconTab(IFormFile IconImage, int rowId)
        {
            int res = 0;
            Tab originalTab = await _DbContext.Tabs.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalTab == null)
            {
                return res;
            }

            if (IconImage != null)
            {
                using var ms = new MemoryStream();
                IconImage.CopyTo(ms);
                originalTab.Icon = ms.ToArray();
            }
            _DbContext.Tabs.Update(originalTab);
            _DbContext.SaveChanges();
            res = originalTab.Id;
            return res;
        }

        public async Task<int> UpdateIconStringTab(string iconString, int rowId)
        {
            int res = 0;
            Tab originalTab = await _DbContext.Tabs.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalTab == null)
            {
                return res;
            }

            originalTab.IconString = iconString;

            _DbContext.Tabs.Update(originalTab);
            _DbContext.SaveChanges();
            res = originalTab.Id;
            return res;
        }

        public async Task<APIResult> DeleteTab(int id, string lang)
        {
            var result = new APIResult();
            Tab tab = await _DbContext.Tabs.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (tab == null)
            {
                return result.FailMe(-1, "Tab not found!", false, APIResult.RESPONSE_CODE.PageNotFound);
            }

            var roleClaim = await _DbContext.RoleClaims.Where(x => x.ClaimType == CustomClaimTypes.Tab && x.ClaimValue == id.ToString()).FirstOrDefaultAsync();

            if (roleClaim != null)
            {
                _DbContext.RoleClaims.Remove(roleClaim);
                await _DbContext.SaveChangesAsync();
            }

            using var transaction = _DbContext.Database.BeginTransaction();
            await _iGeneralRepository.DeleteTranslation(tab.TabNameShortcut);
            _DbContext.Tabs.Remove(tab);
            await _DbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return result.SuccessMe(id, "Tab deleted", false, APIResult.RESPONSE_CODE.PageNotFound);
        }

        public async Task<List<TabGetDto>> GetTabs(string lang)
        {
            var tabs = await _DbContext.Tabs.ToListAsync();
            List<TabGetDto> result = new List<TabGetDto>();

            foreach (var row in tabs)
            {

                TabGetDto tab = new TabGetDto()
                {
                    Id = row.Id,
                    Link = row.Link,
                    ParentId = row.ParentId,
                    TabOrder = row.TabOrder,
                    Icon = row.Icon,
                    IconString = row.IconString,
                    rolesId = _DbContext.RoleClaims.Where(r => r.ClaimValue.Equals(row.Id.ToString())).Select(x=> x.RoleId).ToList(),
                };

                var LangValue = await _iGeneralRepository.getTranslationsForShortCut(row.TabNameShortcut);

                if (LangValue.ContainsKey(lang))
                {
                    tab.Name = LangValue[lang];
                }

                tab.Captions = LangValue;
                result.Add(tab);
            }

            return result;
        }

        public async Task<List<UserTabGetDTO>> GetTabsByIds(List<int> tabIds, string lang)
        {
            List<TabGetDto> result = new List<TabGetDto>();

            var tabs = await _DbContext.Tabs.ToListAsync();

            foreach (var tab in tabs)
            {
                var tabDto = TabGetDto.GetDTO(tab);
                tabDto.HasAccess = tabIds.Contains(tab.Id);
                var langValue = await _iGeneralRepository.getTranslationsForShortCut(tab.TabNameShortcut);
                if (langValue.ContainsKey(lang))
                {
                    tabDto.Name = langValue[lang];
                }
                result.Add(tabDto);
            }

            List<UserTabGetDTO> ordereTabs = new List<UserTabGetDTO>();
            DoRecursive(result, ordereTabs, null);
            return ordereTabs;
        }

        private void DoRecursive(List<TabGetDto> mytabs, List<UserTabGetDTO> res, int? parentId)
        {
            var list = mytabs.Where(p => p.ParentId == parentId).ToList().OrderBy(p => p.TabOrder);
            foreach (var x in list)
            {
                UserTabGetDTO fath = new UserTabGetDTO()
                {
                    Id = x.Id,
                    Link = x.Link,
                    Name = x.Name,
                    TabOrder = x.TabOrder,
                    HasAccess = x.HasAccess,
                    ParentId = x.ParentId,
                    IconBase64 = (x.Icon == null) ? "" : Convert.ToBase64String(x.Icon),
                    IconString = x.IconString,
                    Elements = new List<UserTabGetDTO>()
                };
                res.Add(fath);
                DoRecursive(mytabs, fath.Elements, x.Id);
            }
        }

        public async Task<List<UserTabGetDTO>> GetMyTabs(int userId, string lang)
        {
            var user = await _iUserManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RoleClaims).FirstOrDefaultAsync(u => u.Id == userId);
            List<UserTabGetDTO> res = new List<UserTabGetDTO>();
            if (user == null)
            {
                return res;
            }

            var allTabs = await GetTabs(lang);

            if (!await _iUserManager.IsInRoleAsync(user, Constants.AdminPolicy))
            {
                var userRoles = user.UserRoles;
                List<RoleClaim> userclaims = new List<RoleClaim>();
                foreach (var role in userRoles)
                {
                    userclaims.AddRange(role.Role.RoleClaims.ToList());
                }
                userclaims = userclaims.Distinct().ToList();
                var tabsId = userclaims.Where(x => x.ClaimType == CustomClaimTypes.Tab).Select(x => Int32.Parse(x.ClaimValue)).ToList();
                allTabs = allTabs.Where(x => tabsId.Contains(x.Id)).ToList();
            }

            if (allTabs.Count > 0)
            {
                DoRecursive(allTabs, res, null);
            }

            return res;
        }
    }
}
