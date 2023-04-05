using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories.IUserRepository;

namespace VideoProjectCore6.Services.UserService
{
    public class GroupRepository : IGroupRepository
    {
        private readonly OraDbContext _DbContext;


        public GroupRepository(OraDbContext oraDbContext)
        {
            _DbContext = oraDbContext;
        }

        public async Task<APIResult> AddGroup(UserGroupPostDto userGroupPostDto, int createdBy)
        {
            APIResult result = new APIResult();

            DateTime currentTime = DateTime.UtcNow;

            try
            {

                Group group = new Group
                {
                    GroupName = userGroupPostDto.GroupName,
                    Description = userGroupPostDto.Description,
                    CreatedBy = createdBy,
                    CreatedAt = currentTime,
                    UpdatedAt = currentTime,
                };

                _DbContext.Groups.Add(group);
                await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Added Successfully", false, APIResult.RESPONSE_CODE.CREATED, group);

            }

            catch
            {
                return result.FailMe(-1, "Failed to add a group");
            }
        }

        public async Task<APIResult> DeleteGroup(int groupId, string lang)
        {
            APIResult result = new APIResult();

            DateTime currentTime = DateTime.UtcNow;

            try
            {
                var singleGroup = await _DbContext.Groups.Where(e => e.Id == groupId).FirstOrDefaultAsync();

                if (singleGroup == null)
                {
                    return result.FailMe(-1, "group is not existed");
                }

                _DbContext.Groups.Remove(singleGroup);
                await _DbContext.SaveChangesAsync();


                return result.SuccessMe(1, "Removed Successfully", false, APIResult.RESPONSE_CODE.OK, singleGroup);
            }

            catch
            {
                return result.FailMe(-1, "Failed to remove a group");
            }
        }

        public async Task<APIResult> EditGroup(UserGroupPostDto userGroupPostDto, int groupId, int updatedBy)
        {
            APIResult result = new APIResult();

            DateTime currentTime = DateTime.UtcNow;

            try
            {
                var singleGroup = await _DbContext.Groups.Where(e => e.Id == groupId).FirstOrDefaultAsync();

                if (singleGroup == null)
                {
                    return result.FailMe(-1, "group is not existed");
                }

                singleGroup.GroupName = userGroupPostDto.GroupName;
                singleGroup.Description = userGroupPostDto.Description;
                singleGroup.UpdatedAt = currentTime;

                await EditAcleGroupsAsync(groupId, userGroupPostDto.ACLs, "en");

                _DbContext.Groups.Update(singleGroup);
                await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Updated Successfully", false, APIResult.RESPONSE_CODE.CREATED, singleGroup);

            }

            catch
            {
                return result.FailMe(-1, "Failed to update a group");
            }
        }

        public async Task<ListCount> GetGroups(string text, string name, string lang, int pageSize, int pageIndex)
        {
            var groups = await _DbContext.Groups.Include(i => i.AclGroups).Where(g => (name != null ? g.GroupName.Contains(name) : true)
            && (text != null ? (g.GroupName.ToLower().Contains(text.ToLower()) || g.Description.ToLower().Contains(text.ToLower())) : true)
            ).Select(w => new UserGroupGetDto
            {
                Id = w.Id,
                GroupName = w.GroupName,
                Description = w.Description,
                CreatedAt = w.CreatedAt,
                acls = w.AclGroups.Select(p => new AclsGetDto
                {
                    Id = p.ACL.Id,
                    Name = p.ACL.Name
                }).ToList(),
            }).ToListAsync();


            return new ListCount
            {
                Count = groups.Count(),
                Items = groups.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            };
        }

        public async Task<APIResult> AddUsersToGroup(GroupPostUserDto groupPostUserDto, string lang)
        {
            APIResult result = new APIResult();

            try
            {
                var userIdsSet = groupPostUserDto.userIds.ToHashSet();

                var singleGroup = await _DbContext.Groups.Include(w => w.UserGroups).Where(g => g.Id == groupPostUserDto.groupId).FirstOrDefaultAsync();

                if (singleGroup == null)
                {
                    return result.FailMe(-1, "Group is not existed");
                }

                var existedUserIds = singleGroup.UserGroups.Select(o => o.UserId).ToList();

                foreach (var id in userIdsSet)
                {
                    
                    if (!existedUserIds.Contains(id))
                    {
                        singleGroup.UserGroups.Add(new UserGroup { UserId = id });
                    }
                }

                _DbContext.Groups.Update(singleGroup);

                await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Success to add users to group");

            }
            catch
            {
                return result.FailMe(-1, "Failed to add users to group");
            }
        }

        public async Task<APIResult> RemoveUsersFromGroup(GroupPostUserDto groupPostUserDto, string lang)
        {
            APIResult result = new APIResult();

            try
            {
                var userIdsSet = groupPostUserDto.userIds.ToHashSet();

                var singleGroup = await _DbContext.Groups.Include(w => w.UserGroups).Where(g => g.Id == groupPostUserDto.groupId).FirstOrDefaultAsync();

                if (singleGroup == null)
                {
                    return result.FailMe(-1, "Group is not existed");
                }

                var userGroups = singleGroup.UserGroups.Where(o => userIdsSet.Contains(o.UserId)).ToList();

                _DbContext.UserGroups.RemoveRange(userGroups);
                await _DbContext.SaveChangesAsync();

                return result.SuccessMe(1, "Success to remove users to group");

            }
            catch
            {
                return result.FailMe(-1, "Failed to remove users to group");
            }
        }

        public async Task<ListCount> GetUsersByGroupId(int groupId, int? pageIndex, int? pageSize)
        {
            //APIResult result = new APIResult();

            //try
            //{
                var group = await _DbContext.Groups.Include(w => w.UserGroups).ThenInclude(u => u.User).Where(o => o.Id == groupId).FirstOrDefaultAsync();

                if(group == null)
                {
                  return new ListCount();
                }

                var users = group.UserGroups.Select(l => l.User).ToList().Select(u => new UserCompactView
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email
                }).ToList();

            return new ListCount
            {
                Count = users.Count,
                Items = (pageIndex != null && pageSize != null) ? users.Skip((pageIndex.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault()).Take(pageSize.GetValueOrDefault()) : users,
                };
            //} 
            //catch
            //{
            //    return result.FailMe(-1, "Failed to get users");
            //}
        }



        /// /////////////
        /// /////////////


        public async Task<IdentityResult> EditAcleGroupsAsync(int groupId, List<int> aclGroups, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var group = await _DbContext.Groups.Include(p => p.AclGroups).Where(q => q.Id == groupId).FirstOrDefaultAsync();

            if (group == null)
            {
                throw new InvalidOperationException(Translation.getMessage(lang, "UserNotExistedBefore"));
            }

            var oldgGroups = group.AclGroups;
            await RemoveGroupsFromAclsAsync(oldgGroups);

            List<int> acls = new List<int>();
            acls = await _DbContext.ACLs.Where(x => aclGroups.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            IdentityResult res = IdentityResult.Success;
            if (acls != null)
            {
                foreach (var acl in acls)
                {
                    group.AclGroups.Add(new AclGroups { AclId = acl });
                }
            }

            await _DbContext.SaveChangesAsync();

            scope.Complete();
            return res;
        }

        private async Task RemoveGroupsFromAclsAsync(ICollection<AclGroups> oldGroups)
        {
            foreach (var group in oldGroups)
            {
                _DbContext.AclGroups.Remove(group);
            }

            await _DbContext.SaveChangesAsync();
        }

        /// ////////////////
    }
}
