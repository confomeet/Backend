using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VideoProjectCore6.Models
{
    public partial class OraDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        private readonly IConfiguration _IConfiguration;
        public OraDbContext(IConfiguration iConfiguration)
        {
            _IConfiguration = iConfiguration;
        }

        public OraDbContext(DbContextOptions<OraDbContext> options, IConfiguration iConfiguration)
            : base(options)
        {
            _IConfiguration = iConfiguration;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public virtual DbSet<Action> Actions { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<EventLog> EventLogs { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<ClientInfo> ClientInfos { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<FileConfiguration> FileConfigurations { get; set; } = null!;
        public virtual DbSet<Meeting> Meetings { get; set; } = null!;
        public virtual DbSet<MeetingLogging> MeetingLoggings { get; set; } = null!;

        public virtual DbSet<NotificationAction> NotificationActions { get; set; } = null!;
        public virtual DbSet<NotificationLog> NotificationLogs { get; set; } = null!;
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; } = null!;
        public virtual DbSet<NotificationTemplateDetail> NotificationTemplateDetails { get; set; } = null!;
        public virtual DbSet<OtpLog> OtpLogs { get; set; } = null!;
        public virtual DbSet<Participant> Participants { get; set; } = null!;
        public virtual DbSet<PartyWork> PartyWorks { get; set; } = null!;
        public virtual DbSet<PartyWorkSpeciality> PartyWorkSpecialities { get; set; } = null!;
        public virtual DbSet<QueueProcess> QueueProcesses { get; set; } = null!;
        //public virtual DbSet<Role> Roles { get; set; } = null!;
        //public virtual DbSet<RoleClaim> RoleClaims { get; set; } = null!;
        public virtual DbSet<Speciality> Specialities { get; set; } = null!;
        public virtual DbSet<SysLookupType> SysLookupTypes { get; set; } = null!;
        public virtual DbSet<SysLookupValue> SysLookupValues { get; set; } = null!;
        public virtual DbSet<SysTranslation> SysTranslations { get; set; } = null!;
        public virtual DbSet<Tab> Tabs { get; set; } = null!;
        // public virtual DbSet<User> Users { get; set; } = null!;
        // public virtual DbSet<UserClaim> UserClaims { get; set; } = null!;
        // public virtual DbSet<UserLogin> UserLogins { get; set; } = null!;
        // public virtual DbSet<UserToken> UserTokens { get; set; } = null!;

        public virtual DbSet<Work> Works { get; set; } = null!;

        public virtual DbSet<ConfEvent> ConfEvents { get; set; } = null!;


        public virtual DbSet<ConfUser> ConfUsers { get; set; } = null!;

        public virtual DbSet<RecordingLog> RecordingLogs { get; set; } = null!;


        public virtual DbSet<IndividualsSections> IndividualSections { get; set; } = null!;

        public virtual DbSet<ContactInfo> ContactInfos { get; set; }

        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;

        public virtual DbSet<Group> Groups { get; set; } = null!;

        public virtual DbSet<Files> SysFiles { get; set; } = null!;

        public virtual DbSet<SmtpConfig> SmtpConfigs { get; set; } = null!;

        public virtual DbSet<ACL> ACLs { get; set; } = null!;

        public virtual DbSet<AclGroups> AclGroups { get; set; } = null!;

        public virtual DbSet<S3Recording> S3Recordings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Action>(entity =>
            {
                entity.ToTable("action");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ActionTypeId)
                    .HasPrecision(10)
                    .HasColumnName("action_type_id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.Shortcut)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("shortcut");
            });
            
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("event");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");
                entity.Property(e => e.AppId)
                    .HasPrecision(5)
                    .HasColumnName("app_id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.EGroup)
                      .HasPrecision(10)
                      .HasColumnName("egroup");

                entity.Property(e => e.MeetingId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("meeting_id");

                //entity.Property(e => e.AllDay)
                //    .HasMaxLength(20)
                //    .IsUnicode(false)
                //    .HasColumnName("MEETING_ID");

                entity.Property(e => e.Type)
                    .HasPrecision(5)
                    .HasColumnName("type");
                entity.Property(e => e.OrderNo)
                    .HasPrecision(10)
                    .HasColumnName("order_no")
                    .HasDefaultValueSql("1 ");
                entity.Property(e => e.Organizer)
                      .HasMaxLength(100)
                      .HasColumnName("organizer");
                entity.Property(e => e.ParentEvent)
                    .HasPrecision(10)
                    .HasColumnName("parent_event");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.TimeZone)
                      .HasMaxLength(50)
                      .IsUnicode(false)
                      .HasColumnName("time_zone");
                entity.Property(e => e.SubTopic)
                      .HasMaxLength(255)
                      .HasColumnName("sub_topic");

                entity.Property(e => e.Topic)
                    .HasMaxLength(255)
                    .HasColumnName("topic");

                entity.Property(e => e.AllDay)
                      .HasColumnName("all_day");


                entity.HasOne(d => d.Meeting)
                   .WithMany(p => p.Events)
                   .HasPrincipalKey(p => p.MeetingId)
                   .HasForeignKey(d => d.MeetingId)
                   .HasConstraintName("event_meeting_id_fk");

                entity.HasOne(d => d.User)
                   .WithMany(p => p.Events)
                  // .HasPrincipalKey(p => p.CreateBy)
                   .HasForeignKey(d => d.CreatedBy)
                   .HasConstraintName("fk_user_");

                entity.HasOne(d => d.ParentEventNavigation)
                   .WithMany(p => p.InverseParentEventNavigation)
                   .HasForeignKey(d => d.ParentEvent)
                   .HasConstraintName("event_parent_event_id_fk");


            });

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.ToTable("event_log");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ActionId)
                    .HasPrecision(5)
                    .HasColumnName("action_id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.EventId)
                    .HasPrecision(10)
                    .HasColumnName("event_id");

                entity.Property(e => e.Note)
                    .HasMaxLength(4000)
                    .HasColumnName("note");

                entity.Property(e => e.ObjectType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("object_type");

                entity.Property(e => e.RelatedId)
                    .HasPrecision(10)
                    .HasColumnName("related_id");
            });

            modelBuilder.Entity<ClientInfo>(entity =>
            {
                entity.ToTable("client_info");

                entity.Property(e => e.Id)
                    .HasPrecision(5)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.AppKey)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("app_key");

                entity.Property(e => e.AppName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("app_name");

                entity.Property(e => e.ClientName)
                    .HasMaxLength(200)
                    .HasColumnName("client_name");

                entity.Property(e => e.IsActive)
                    .HasPrecision(1)
                    .HasColumnName("is_active");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");
            });

            modelBuilder.Entity<RecordingLog>(entity =>
            {
                entity.ToTable("recording_log");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.RecordingfileName)
                     .HasPrecision(10)
                    .HasColumnName("recording_file_name");

                entity.Property(e => e.FileSize)
                     .HasPrecision(10)
                    .HasColumnName("file_size");

                entity.Property(e => e.IsSucceeded)
                    .HasPrecision(2)
                    .HasColumnName("is_succeeded");

                //entity.Property(e => e.RecordingDate)
                //    .HasColumnType("DATE")
                //    .HasColumnName("RECORDING_DATE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.FilePath)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("file_path");

                entity.Property(e => e.VideoPublicLink)
                    .HasColumnName("video_public_link");

                entity.Property(e => e.Status)
                    .HasPrecision(2)
                    .HasColumnName("status")
                    .HasConversion<int>();

                entity.Property(e => e.UploadDate)
                    .HasColumnName("upload_date")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("contact");
                entity.HasIndex(e => new { e.UserId, e.ContactId }, "contact_user_id_contact_id_uk")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ContactId)
                    .HasPrecision(10)
                    .HasColumnName("contact_id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(150)
                    .HasColumnName("display_name");

                entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");

                entity.Property(e => e.Home)
                      .HasMaxLength(25)
                      .IsUnicode(false)
                      .HasColumnName("home");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpddatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_upddated_date");
                entity.Property(e => e.Mobile)
                      .HasMaxLength(25)
                      .IsUnicode(false)
                      .HasColumnName("mobile");

                entity.Property(e => e.ImageUrl)
                      .HasMaxLength(200)
                      .IsUnicode(false)
                      .HasColumnName("image_url");

                entity.Property(e => e.Office)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("office");
                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                // -------------------

                entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");

                entity.Property(e => e.JobDesc)
                      .HasMaxLength(25)
                      .IsUnicode(false)
                      .HasColumnName("job_desc");

                entity.Property(e => e.Country)
                .HasPrecision(10)
                .HasColumnName("country");

                entity.Property(e => e.Address)
                      .HasMaxLength(25)
                      .IsUnicode(false)
                      .HasColumnName("address");

                entity.Property(e => e.City)
                      .HasMaxLength(50)
                      .IsUnicode(false)
                      .HasColumnName("city");

                entity.Property(e => e.Website)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("website");

                entity.Property(e => e.Specialization)
                      .HasMaxLength(25)
                      .IsUnicode(false)
                      .HasColumnName("specialization");


                entity.Property(e => e.DirectManageId)
                      .HasPrecision(10)
                      .HasColumnName("direct_manager_id");


                entity.Property(e => e.CompanyId)
                .HasPrecision(10)
                 .HasColumnName("company_id");

                entity.Property(e => e.SectionId)
                      .HasPrecision(10)
                      .HasColumnName("section_id");

                entity.Property(e => e.ShareWith)
                      .HasPrecision(1)
                      .HasColumnName("share_with");


                // -----------------------

                entity.HasOne(d => d.ContactNavigation)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contact_contact_id_fk");


                entity.HasOne(d => d.ParentEventNavigation)
                   .WithMany(p => p.InverseParentEventNavigation)
                   .HasForeignKey(d => d.DirectManageId)
                   .HasConstraintName("fk_contact_3");

                entity.HasOne(d => d.ParentCompanyId)
                   .WithMany(p => p.Companies)
                   .HasForeignKey(d => d.CompanyId)
                   .HasConstraintName("fk_contact_2");

                entity.HasOne(d => d.ParentSectionId)
                   .WithMany(p => p.InverseParentSectionId)
                   .HasForeignKey(d => d.SectionId)
                   .HasConstraintName("fk_contact");

            });

            modelBuilder.Entity<ContactInfo>(entity =>
            {
                entity.ToTable("contact_info");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ContactId)
                     .HasPrecision(10)
                    .HasColumnName("contact_id");

                entity.Property(e => e.InfoType)
                     .HasPrecision(10)
                    .HasColumnName("info_type");

                entity.Property(e => e.InfoValue)
                     .HasPrecision(10)
                    .HasColumnName("info_value");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CntId)
                    .HasName("country_pk");

                entity.ToTable("country");

                entity.Property(e => e.CntId)
                    .HasPrecision(10)
                    .HasColumnName("cnt_id");

                entity.Property(e => e.CntCapitalAr)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_capital_ar");

                entity.Property(e => e.CntCapitalEn)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_capital_en");

                entity.Property(e => e.CntConIdFk)
                    .HasPrecision(10)
                    .HasColumnName("cnt_con_id_fk");

                entity.Property(e => e.CntContinentAr)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_continent_ar");

                entity.Property(e => e.CntContinentEn)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_continent_en");

                entity.Property(e => e.CntCountryAr)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_country_ar");

                entity.Property(e => e.CntCountryEn)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_country_en");

                entity.Property(e => e.CntGlobalCode)
                    .HasMaxLength(16)
                    .HasColumnName("cnt_global_code");

                entity.Property(e => e.CntIso2)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_iso2");

                entity.Property(e => e.CntIso3)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_iso3");

                entity.Property(e => e.CntOfficialNameAr)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_official_name_ar");

                entity.Property(e => e.CntOfficialNameEn)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_official_name_en");

                entity.Property(e => e.CntRegIdFk)
                    .HasPrecision(10)
                    .HasColumnName("cnt_reg_id_fk");

                entity.Property(e => e.CntRegionAr)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_region_ar");

                entity.Property(e => e.CntRegionEn)
                    .HasMaxLength(64)
                    .HasColumnName("cnt_region_en");

                entity.Property(e => e.UgId)
                    .HasPrecision(10)
                    .HasColumnName("ug_id");
            });

            modelBuilder.Entity<ConfEvent>(entity =>
            {
                entity.ToTable("conf_event");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.EventTime)
                      .HasColumnType("datetime")
                      .HasColumnName("event_time");

                entity.Property(e => e.EventType)
                .HasPrecision(10)
                .HasColumnName("event_type");

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .HasColumnName("user_id");

                entity.Property(e => e.EventInfo)
                    .HasMaxLength(1000)
                    .HasColumnName("event_info");


                entity.Property(e => e.MeetingId)
                    .HasMaxLength(30)
                    .HasColumnName("meeting_id");
            });

            modelBuilder.Entity<ConfUser>(entity =>
            {
                entity.ToTable("conf_user");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.Avatar)
                      .HasMaxLength(100)
                      .HasColumnName("avatar");

                entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

                entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");


                entity.Property(e => e.ProsodyId)
                    .HasMaxLength(100)
                    .HasColumnName("prosody_id");

                entity.Property(e => e.ConfId)
                    .HasMaxLength(100)
                    .HasColumnName("conf_id");

                entity.Property(e => e.ConfTime)
                    .HasColumnType("datetime")
                    .HasColumnName("conf_time");
            });

            modelBuilder.Entity<FileConfiguration>(entity =>
            {
                entity.ToTable("file_configuration");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Extension)
                    .HasMaxLength(50)
                    .HasColumnName("extension");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.MaxSize)
                    .HasPrecision(10)
                    .HasColumnName("max_size");

                entity.Property(e => e.MinSize)
                    .HasPrecision(10)
                    .HasColumnName("min_size");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.ToTable("meeting");
                entity.HasIndex(e => e.MeetingId, "meeting_meeting_id_uk")
                      .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.MeetingId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("meeting_id");

                entity.Property(e => e.MeetingLog)
                    .HasMaxLength(1000)
                    .HasColumnName("meeting_log");

                entity.Property(e => e.EventId)
                    .HasPrecision(10)
                    .HasColumnName("event_id");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.PasswordReq)
                    .HasColumnName("password_req");

                entity.Property(e => e.RecordingReq)
                      .HasColumnName("recording_req");

                entity.Property(e => e.SingleAccess)
                      .HasColumnName("single_access");

                entity.Property(e => e.AutoLobby)
                      .HasColumnName("auto_lobby");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status")
                    .HasDefaultValueSql("null");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.Status)
                    .HasPrecision(2)
                    .HasColumnName("status");

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(50)
                    .HasColumnName("time_zone");

                entity.Property(e => e.Topic)
                    .HasMaxLength(200)
                    .HasColumnName("topic");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

            });


            modelBuilder.Entity<MeetingLogging>(entity =>
            {
                entity.ToTable("meeting_logging");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.FirstLogin)
                    .HasColumnType("datetime")
                    .HasColumnName("first_login");

                entity.Property(e => e.IsModerator)
                    .HasPrecision(1)
                    .HasColumnName("is_moderator");

                entity.Property(e => e.LoginDate)
                    .HasColumnType("datetime")
                    .HasColumnName("login_date");

                entity.Property(e => e.MeetingId)
                    .HasPrecision(10)
                    .HasColumnName("meeting_id");

                entity.Property(e => e.PreviousLoginList)
                    .HasMaxLength(1000)
                    .HasColumnName("previous_login_list");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.MeetingLoggings)
                    .HasForeignKey(d => d.MeetingId)
                    .HasConstraintName("meeting_logging_meeting_id_fk");
            });

            modelBuilder.Entity<NotificationAction>(entity =>
            {
                entity.ToTable("notification_action");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ActionId)
                    .HasPrecision(10)
                    .HasColumnName("action_id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.NotificationTemplateId)
                    .HasPrecision(10)
                    .HasColumnName("notification_template_id");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.NotificationActions)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("notification_action_action_id_fk");

                entity.HasOne(d => d.NotificationTemplate)
                    .WithMany(p => p.NotificationActions)
                    .HasForeignKey(d => d.NotificationTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("notification_action_notification_id_fk");
            });

            modelBuilder.Entity<NotificationLog>(entity =>
            {
                entity.ToTable("notification_log");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ApplicationId)
                    .HasPrecision(10)
                    .HasColumnName("application_id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Hostsetting).HasColumnName("hostsetting");

                entity.Property(e => e.IsSent)
                    .HasPrecision(2)
                    .HasColumnName("is_sent");

                entity.Property(e => e.Lang)
                    .HasMaxLength(5)
                    .HasColumnName("lang");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");
                entity.Property(e => e.LinkCaption).HasColumnName("link_caption");
                entity.Property(e => e.NotificationBody).HasColumnName("notification_body");
                entity.Property(e => e.NotificationLink).HasColumnName("notification_link");

                entity.Property(e => e.NotificationChannelId)
                    .HasPrecision(10)
                    .HasColumnName("notification_channel_id");

                entity.Property(e => e.NotificationTitle).HasColumnName("notification_title");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.ReportValueId)
                    .HasMaxLength(1000)
                    .HasColumnName("report_value_id");

                entity.Property(e => e.SendReportId).HasColumnName("send_report_id");

                entity.Property(e => e.SentCount)
                    .HasPrecision(3)
                    .HasColumnName("sent_count");

                entity.Property(e => e.ToAddress).HasColumnName("to_address");
                entity.Property(e => e.Template).HasColumnName("template");
                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.ToTable("notification_template");

                entity.HasIndex(e => e.NotificationNameShortcut, "notification_template_shortcut_un")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.NotificationNameShortcut)
                    .HasMaxLength(250)
                    .HasColumnName("notification_name_shortcut");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");
            });

            modelBuilder.Entity<NotificationTemplateDetail>(entity =>
            {
                entity.ToTable("notification_template_detail");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.BodyShortcut)
                    .HasMaxLength(250)
                    .HasColumnName("body_shortcut");

                entity.Property(e => e.ChangeAble)
                    .HasPrecision(1)
                    .HasColumnName("change_able");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.NotificationChannelId)
                    .HasPrecision(10)
                    .HasColumnName("notification_channel_id");

                entity.Property(e => e.NotificationTemplateId)
                    .HasPrecision(10)
                    .HasColumnName("notification_template_id");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.TitleShortcut)
                    .HasMaxLength(250)
                    .HasColumnName("title_shortcut");

                entity.HasOne(d => d.NotificationTemplate)
                    .WithMany(p => p.NotificationTemplateDetails)
                    .HasForeignKey(d => d.NotificationTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_notification_template_detail_notification_template_id");
            });

            modelBuilder.Entity<OtpLog>(entity =>
            {
                entity.ToTable("otp_log");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.GeneratedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("generated_date");

                entity.Property(e => e.OtpCode)
                    .HasMaxLength(6)
                    .HasColumnName("otp_code")
                    .IsFixedLength();

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OtpLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("otp_log_user_id_fk");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("participant");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");
                entity.Property(e => e.Charge)
                    .HasMaxLength(500)
                    .HasColumnName("charge");
                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");
                entity.Property(e => e.Email)
                     .HasMaxLength(100)
                     .IsUnicode(false)
                     .HasColumnName("email");
                entity.Property(e => e.Description)
                      .HasMaxLength(200)
                      .HasColumnName("description");
                entity.Property(e => e.EventId)
                    .HasPrecision(10)
                    .HasColumnName("event_id");
                entity.Property(e => e.IsModerator)
                    .HasPrecision(1)
                    .HasColumnName("is_moderator");
                entity.Property(e => e.GroupIn)
                    .HasPrecision(10)
                    .HasColumnName("groupin");
                entity.Property(e => e.Guid).HasColumnName("guid");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.MeetingToken)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("meeting_token");
                entity.Property(e => e.Mobile)
                      .HasMaxLength(25)
                      .IsUnicode(false)
                      .HasColumnName("mobile");

                entity.Property(e => e.Note)
                    .HasMaxLength(200)
                    .HasColumnName("note");
                entity.Property(e => e.PartyId)
                      .HasPrecision(10)
                      .HasColumnName("party_id");
                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");
                entity.Property(e => e.UserType)
                      .HasPrecision(2)
                      .HasColumnName("user_type");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("participant_event_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("participant_user_id_fk");
            });

            modelBuilder.Entity<PartyWork>(entity =>
            {
                entity.ToTable("party_work");

                entity.HasIndex(e => new { e.PartyId, e.WorkId }, "party_work_uk")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.PartyId)
                    .HasPrecision(10)
                    .HasColumnName("party_id");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.WorkId)
                    .HasPrecision(10)
                    .HasColumnName("work_id");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.PartyWorks)
                    .HasForeignKey(d => d.PartyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("party_work_party_id_user_id_fk");

                entity.HasOne(d => d.Work)
                    .WithMany(p => p.PartyWorks)
                    .HasForeignKey(d => d.WorkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("party_work_work_id");
            });

            modelBuilder.Entity<PartyWorkSpeciality>(entity =>
            {
                entity.ToTable("party_work_speciality");

                entity.HasIndex(e => new { e.PartyWorkId, e.SpecialityId }, "party_work_speciality_uk")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("date")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.PartyWorkId)
                    .HasPrecision(10)
                    .HasColumnName("party_work_id");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.SpecialityId)
                    .HasPrecision(10)
                    .HasColumnName("speciality_id");

                entity.HasOne(d => d.PartyWork)
                    .WithMany(p => p.PartyWorkSpecialities)
                    .HasForeignKey(d => d.PartyWorkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("party_work_speciality_party_work_id_fk");

                entity.HasOne(d => d.Speciality)
                    .WithMany(p => p.PartyWorkSpecialities)
                    .HasForeignKey(d => d.SpecialityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("party_work_speciality_speciality_id_fk");
            });

            modelBuilder.Entity<QueueProcess>(entity =>
            {
                entity.ToTable("queue_processes");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.ExpectedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("expected_date_time");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.Note)
                    .HasMaxLength(200)
                    .HasColumnName("note");

                entity.Property(e => e.NotifyHighLevel)
                    .HasPrecision(1)
                    .HasColumnName("notify_high_level");

                entity.Property(e => e.NotifyLowLevel)
                    .HasPrecision(1)
                    .HasColumnName("notify_low_level");

                entity.Property(e => e.NotifyMediumLevel)
                    .HasPrecision(1)
                    .HasColumnName("notify_medium_level");

                entity.Property(e => e.ProcessNo)
                    .HasPrecision(10)
                    .HasColumnName("process_no");

                entity.Property(e => e.Provider)
                    .HasMaxLength(50)
                    .HasColumnName("provider");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.ServiceKindNo)
                    .HasPrecision(10)
                    .HasColumnName("service_kind_no");

                entity.Property(e => e.Status)
                    .HasPrecision(2)
                    .HasColumnName("status");

                entity.Property(e => e.TicketId)
                    .HasPrecision(10)
                    .HasColumnName("ticket_id");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ConcurrencyStamp)
                    .HasMaxLength(500)
                    .HasColumnName("concurrency_stamp");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.NormalizedName)
                    .HasMaxLength(100)
                    .HasColumnName("normalized_name");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");
            });

            modelBuilder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable("role_claim");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ClaimType)
                    .HasMaxLength(200)
                    .HasColumnName("claim_type");

                entity.Property(e => e.ClaimValue)
                    .HasMaxLength(200)
                    .HasColumnName("claim_value");

                entity.Property(e => e.RoleId)
                    .HasPrecision(10)
                    .HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("role_claim_role_id_fk");
            });

            modelBuilder.Entity<Speciality>(entity =>
            {
                entity.ToTable("speciality");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.Shortcut)
                    .HasMaxLength(150)
                    .HasColumnName("shortcut");

                entity.Property(e => e.WorkId)
                    .HasPrecision(10)
                    .HasColumnName("work_id");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SpecialityCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("specialty_created_by_fk");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.SpecialityLastUpdatedByNavigations)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("specialty_last_updated_by_fk");

                entity.HasOne(d => d.Work)
                    .WithMany(p => p.Specialities)
                    .HasForeignKey(d => d.WorkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("specialty_work_id_fk");
            });

            modelBuilder.Entity<SysLookupType>(entity =>
            {
                entity.ToTable("sys_lookup_type");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.Value)
                    .HasMaxLength(250)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<SysLookupValue>(entity =>
            {
                entity.ToTable("sys_lookup_value");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.BoolParameter)
                    //.HasPrecision(1)
                    .HasColumnName("bool_parameter");

                entity.Property(e => e.LookupTypeId)
                    .HasPrecision(10)
                    .HasColumnName("lookup_type_id");

                entity.Property(e => e.Order)
                    .HasPrecision(3)
                    .HasColumnName("order_");

                entity.Property(e => e.Shortcut)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("shortcut");

                entity.Property(e => e.Specification)
                    .HasMaxLength(100)
                    .HasColumnName("specification");

                entity.HasOne(d => d.LookupType)
                    .WithMany(p => p.SysLookupValues)
                    .HasForeignKey(d => d.LookupTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lookup_value_lookup_type_id");
            });

            modelBuilder.Entity<SysTranslation>(entity =>
            {
                entity.ToTable("sys_translation");

                entity.HasIndex(e => new { e.Shortcut, e.Lang }, "sys_translation_shortcut_lang_un")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.Lang)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("lang");

                entity.Property(e => e.Shortcut)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("shortcut");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Tab>(entity =>
            {
                entity.ToTable("tab");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.Icon)
                    .HasColumnType("blob")
                    .HasColumnName("icon");

                entity.Property(e => e.IconString)
                    .HasMaxLength(250)
                    .HasColumnName("icon_string");

                entity.Property(e => e.Link)
                    .HasMaxLength(250)
                    .HasColumnName("link");

                entity.Property(e => e.ParentId)
                    .HasPrecision(10)
                    .HasColumnName("parent_id");

                entity.Property(e => e.TabNameShortcut)
                    .HasMaxLength(250)
                    .HasColumnName("tab_name_shortcut");

                entity.Property(e => e.TabOrder)
                    .HasPrecision(3)
                    .HasColumnName("tab_order");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("fk_tab_tab_parent_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user_");
                entity.HasIndex(e => new { e.UserId, e.UserType }, "USER_USERTYPE_USERID_UK")
                   .IsUnique();
                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.AccessFailedCount)
                    .HasPrecision(10)
                    .HasColumnName("access_failed_count");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.AreaId)
                    .HasPrecision(10)
                    .HasColumnName("area_id");

                entity.Property(e => e.BirthdayDate)
                    .HasColumnType("datetime")
                    .HasColumnName("birthday_date");

                entity.Property(e => e.City)
                    .HasPrecision(2)
                    .HasColumnName("city");

                entity.Property(e => e.ConcurrencyStamp)
                    .HasMaxLength(200)
                    .HasColumnName("concurrency_stamp");

                entity.Property(e => e.CreatedDate)
                    //.HasColumnType("date")
                    .HasColumnName("created_date");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .HasColumnName("email");

                entity.Property(e => e.EmailConfirmed)
                    .HasColumnName("email_confirmed");

                entity.Property(e => e.EmailLang)
                    .HasMaxLength(50)
                    .HasColumnName("email_lang");

                entity.Property(e => e.Emirate)
                    .HasPrecision(2)
                    .HasColumnName("emirate");

                entity.Property(e => e.EmiratesId)
                    .HasMaxLength(25)
                    .HasColumnName("emirates_id");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnType("date")
                    .HasColumnName("end_effective_date");

                entity.Property(e => e.EntityId)
                    .HasPrecision(10)
                    .HasColumnName("entity_id");

                entity.Property(e => e.FullName)
                    .HasMaxLength(200)
                    .HasColumnName("full_name");

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .HasColumnName("gender");

                entity.Property(e => e.Image)
                    .HasMaxLength(200)
                    .HasColumnName("image");

                entity.Property(e => e.IsEntity)
                    .HasColumnName("is_entity");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("date")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.LockoutEnabled)

                    .HasColumnName("lockout_enabled");

                entity.Property(e => e.LockoutEnd)
                    .HasPrecision(7)
                    .HasColumnName("lockout_end");

                entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime")
                .HasColumnName("lockoutenddateutc"); ;

                entity.Property(e => e.ManagerId)
                    .HasPrecision(10)
                    .HasColumnName("manager_id");

                entity.Property(e => e.NatId)
                    .HasPrecision(10)
                    .HasColumnName("nat_id");

                entity.Property(e => e.NormalizedEmail)
                    .HasMaxLength(200)
                    .HasColumnName("normalized_email");

                entity.Property(e => e.NormalizedUserName)
                    .HasMaxLength(200)
                    .HasColumnName("normalized_user_name");

                entity.Property(e => e.NotificationType)
                    .HasPrecision(10)
                    .HasColumnName("notification_type");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(200)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(200)
                    .HasColumnName("phone_number");

                entity.Property(e => e.PhoneNumberConfirmed)
                    .HasPrecision(1)
                    .HasColumnName("phone_number_confirmed");

                entity.Property(e => e.ProfileStatus)
                    .HasPrecision(10)
                    .HasColumnName("profile_status");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.SecurityQuestionAnswer)
                    .HasMaxLength(100)
                    .HasColumnName("security_question_answer");

                entity.Property(e => e.SecurityQuestionId)
                    .HasPrecision(10)
                    .HasColumnName("security_question_id");

                entity.Property(e => e.SecurityStamp)
                    .HasMaxLength(500)
                    .HasColumnName("security_stamp");

                entity.Property(e => e.Sign)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("sign");

                entity.Property(e => e.SmsLang)
                    .HasMaxLength(50)
                    .HasColumnName("sms_lang");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnType("date")
                    .HasColumnName("start_effective_date");

                entity.Property(e => e.Status)
                    .HasPrecision(10)
                    .HasColumnName("status");

                entity.Property(e => e.TelNo)
                    .HasMaxLength(50)
                    .HasColumnName("tel_no");

                entity.Property(e => e.TwoFactorEnabled)
                    .HasPrecision(1)
                    .HasColumnName("two_factor_enabled");
                entity.Property(e => e.UserId)
                      .HasPrecision(10)
                      .HasColumnName("user_id");
                entity.Property(e => e.UserName)
                    .HasMaxLength(200)
                    .HasColumnName("user_name");
                entity.Property(e => e.UserType)
                      .HasPrecision(2)
                      .HasColumnName("user_type");
                entity.Property(e => e.UuId)
                      .HasMaxLength(25)
                      .HasColumnName("uuid");

                entity.Property(e => e.Website)
                    .HasMaxLength(100)
                    .HasColumnName("website");

                //entity.Property(e => e.ReCaptchaToken)
                //.HasMaxLength(1000)
                //.HasColumnName("RE_CAPTCHA_TOKEN");

                entity.Property(e => e.meetingId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("meeting_id");

                /*entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("USER_ROLE_ROLE_ID_FK"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("USER_ROLE_USER_ID_FK"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("user_role_user_id_role_id_PK");

                            j.ToTable("user_role");

                            j.IndexerProperty<int>("UserId").HasPrecision(10).HasColumnName("user_id");

                            j.IndexerProperty<int>("RoleId").HasPrecision(10).HasColumnName("role_id");
                        });*/
            });

            modelBuilder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("user_claim");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.ClaimType)
                    .HasMaxLength(100)
                    .HasColumnName("claim_type");

                entity.Property(e => e.ClaimValue)
                    .HasMaxLength(100)
                    .HasColumnName("claim_value");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClaims)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_claim_user_id");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("user_login");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.BrowserName)
                    .HasMaxLength(100)
                    .HasColumnName("browser_name");

                entity.Property(e => e.BrowserVersion)
                    .HasMaxLength(100)
                    .HasColumnName("browser_version");

                entity.Property(e => e.Device)
                    .HasMaxLength(500)
                    .HasColumnName("device");

                entity.Property(e => e.Ip)
                    .HasMaxLength(20)
                    .HasColumnName("ip");

                entity.Property(e => e.LoginDate)
                    .HasColumnType("datetime")
                    .HasColumnName("login_date");

                entity.Property(e => e.LoginProvider)
                    .HasMaxLength(128)
                    .HasColumnName("login_provider");

                entity.Property(e => e.Os)
                    .HasMaxLength(50)
                    .HasColumnName("os");

                entity.Property(e => e.ProviderDisplayName)
                    .HasMaxLength(128)
                    .HasColumnName("provider_display_name");

                entity.Property(e => e.ProviderKey)
                    .HasMaxLength(128)
                    .HasColumnName("provider_key");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogins)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_loging_user_id");
            });

            modelBuilder.Entity<IndividualsSections>(entity =>
            {
                entity.HasKey(e => new { e.IndividualId, e.SectionId })
                    .HasName("individuals_sections_pk");

                entity.ToTable("individuals_sections");

                entity.Property(e => e.IndividualId)
                    .HasPrecision(10)
                    .HasColumnName("individual_id");

                entity.Property(e => e.SectionId)
                    .HasPrecision(10)
                    .HasColumnName("section_id");

                entity.HasOne(d => d.Individual)
                    .WithMany(p => p.IndividualsSections)
                    .HasForeignKey(d => d.IndividualId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_individual_section");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.IndividualSections)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_section_individaul");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("user_role_user_id_role_id_pk");

                entity.ToTable("user_role");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                entity.Property(e => e.RoleId)
                    .HasPrecision(10)
                    .HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_role_id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_user_id_fk");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.ToTable("user_group");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");
                
                entity.Property(e => e.GroupId)
                    .HasPrecision(10)
                    .HasColumnName("group_id");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UserGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_group_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserGroups)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_group_fk_1");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("group");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.GroupName)
                    .HasColumnName("group_name");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    //.HasColumnType("datetime")
                    .HasColumnName("created_by");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("user_token");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.LoginProvider)
                    .HasMaxLength(150)
                    .HasColumnName("login_provider");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.UserId)
                    .HasPrecision(10)
                    .HasColumnName("user_id");

                entity.Property(e => e.Value)
                    .HasMaxLength(150)
                    .HasColumnName("value");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_token_user_id");
            });

            modelBuilder.Entity<Work>(entity =>
            {
                entity.ToTable("work");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasPrecision(10)
                    .HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.LastUpdatedBy)
                    .HasPrecision(10)
                    .HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_updated_date");

                entity.Property(e => e.RecStatus)
                    .HasPrecision(2)
                    .HasColumnName("rec_status");

                entity.Property(e => e.Shorcut)
                    .HasMaxLength(250)
                    .HasColumnName("shorcut");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.WorkCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("work_created_by_fk");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.WorkLastUpdatedByNavigations)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("work_last_updated_by_fk");
            });

            modelBuilder.Entity<Files>(entity =>
            {
                entity.ToTable("file");

                entity.HasIndex(e => e.Id, "SYS_C008756")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.FileName)
                    .HasColumnType("varchar2")
                    .HasMaxLength(255)
                    .HasColumnName("fileName");

                entity.Property(e => e.FilePath)
                    .HasColumnType("varchar2")
                    .HasMaxLength(255)
                    .HasColumnName("filePath");

                entity.Property(e => e.FileSize)
                    .HasColumnName("fileSize");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.ContactId).HasColumnName("contactId");


                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPhotos)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("file_fk");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.UserPhotos)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("file_fk_1");
            });

            modelBuilder.Entity<SmtpConfig>(entity =>
            {
                entity.ToTable("smtp_config");

                entity.HasIndex(e => e.Id, "SYS_C008756")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.DisplayName)
                    .HasColumnType("varchar2")
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar2")
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Host)
                    .HasColumnName("host");


                entity.Property(e => e.Port)
                    .HasColumnName("port");

                entity.Property(e => e.Encryption)
                    .HasColumnName("encryption");

                entity.Property(e => e.Secure)
                    .HasColumnType("varchar2")
                    .HasMaxLength(255)
                    .HasColumnName("secure");

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name");


                entity.Property(e => e.Password)
                    .HasColumnName("password");

                entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                    .HasColumnName("updated_at");


                entity.Property(e => e.CreatedById)
     
                    .HasColumnName("created_by_id");

                entity.Property(e => e.UpdatedById)

                    .HasColumnName("updated_by_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SmtpConfigs)
                    .HasForeignKey(d => d.CreatedById)
                    .HasConstraintName("smtp_config_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SmtpConfigs)
                    .HasForeignKey(d => d.UpdatedById)
                    .HasConstraintName("smtp_config_fk_1");

            });

            modelBuilder.Entity<AclGroups>(entity =>
            {
                entity.ToTable("acl_user_groups_user_group");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.AclId)
                    .HasColumnName("acl_id");

                entity.Property(e => e.UserGroupId)
                    .HasPrecision(10)
                    .HasColumnName("user_group_id");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AclGroups)
                    .HasForeignKey(d => d.UserGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("acl_user_groups_user_group_fk");

                entity.HasOne(d => d.ACL)
                    .WithMany(p => p.AclGroups)
                    .HasForeignKey(d => d.AclId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_acl_group_acl");
            });

            modelBuilder.Entity<ACL>(entity =>
            {
                entity.ToTable("acl");

                entity.Property(e => e.Id)
                    .HasPrecision(10)
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name");

                entity.Property(e => e.CreatedById)
                    .HasColumnName("createdById");

                entity.Property(e => e.UpdatedById)
                    .HasColumnName("updatedById");

            });

            modelBuilder.Entity<S3Recording>(entity =>
            {
                entity.ToTable("s3_recordings");

                entity.Property(e => e.Uuid)
                    .HasColumnName("uuid");

                entity.Property(e => e.RecordingLog)
                    .HasColumnName("recording_log");

                entity.Property(e => e.FileName)
                    .HasColumnName("file_name");

                entity.Property(e => e.FileSize)
                    .HasColumnName("file_size");

                entity.Property(e => e.Bucket)
                    .HasColumnName("bucket");

                entity.Property(e => e.Key)
                    .HasColumnName("key");
            });

            modelBuilder.HasSequence("sequence_login");

            modelBuilder.HasSequence("sequenceformeetingid");

            modelBuilder.HasSequence("general_seq");

            modelBuilder.HasSequence("egroup_seq");

        }
    }
}
