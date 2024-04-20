using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

#nullable disable
namespace VideoProjectCore6.Models
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {

            OtpLogs = new HashSet<OtpLog>();
            UserClaims = new HashSet<UserClaim>();
            UserLogins = new HashSet<UserLogin>();
            UserTokens = new HashSet<UserToken>();
            UserRoles = new HashSet<UserRole>();
            Roles = new HashSet<Role>();
            Participants = new HashSet<Participant>();
            Contacts = new HashSet<Contact>();
            Events = new HashSet<Event>();
            UserGroups = new HashSet<UserGroup>();
            UserPhotos = new HashSet<Files>();
            SmtpConfigs = new HashSet<SmtpConfig>();
        }

        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public int? SecurityQuestionId { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Gender { get; set; }
        public int? NatId { get; set; }
        public string TelNo { get; set; }
        public string EmailLang { get; set; }
        public string SmsLang { get; set; }
        public int? AreaId { get; set; }
        public int? NotificationType { get; set; }
        public int? ProfileStatus { get; set; }
        public string Address { get; set; }
        public int? CountryId { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public byte? RecStatus { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public string Sign { get; set; }
        public string meetingId { get; set; }

        //[Required]
        //public string ReCaptchaToken { get; set; }

        public virtual ICollection<OtpLog> OtpLogs { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; }
        public virtual ICollection<UserLogin> UserLogins { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Files> UserPhotos { get; set; }
        public virtual ICollection<SmtpConfig> SmtpConfigs { get; set; }
        public virtual Country Country { get; set; }
    }
}
