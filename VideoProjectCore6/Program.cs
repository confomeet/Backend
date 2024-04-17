using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using VideoProjectCore6;
using VideoProjectCore6.DTOs.ChannelDto;
using VideoProjectCore6.Models;
using VideoProjectCore6.Repositories;
using VideoProjectCore6.Repositories.IConfEventRepository;
using VideoProjectCore6.Repositories.IContactRepository;
using VideoProjectCore6.Repositories.ICountryRepository;
using VideoProjectCore6.Repositories.IEventLogRepository;
using VideoProjectCore6.Repositories.IEventRepository;
using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.Repositories.IMeetingRepository;
using VideoProjectCore6.Repositories.INotificationRepository;
using VideoProjectCore6.Repositories.IParticipantRepository;
using VideoProjectCore6.Repositories.IRecordingRepository;
using VideoProjectCore6.Repositories.IRoleRepository;
using VideoProjectCore6.Repositories.IStatisticsRepository;
using VideoProjectCore6.Repositories.ITabRepository;
using VideoProjectCore6.Repositories.IUserRepository;
using VideoProjectCore6.Services;
using VideoProjectCore6.Services.ConfEventService;
using VideoProjectCore6.Services.ContactService;
using VideoProjectCore6.Services.CountryService;
using VideoProjectCore6.Services.Event;
using VideoProjectCore6.Services.EventLog;
using VideoProjectCore6.Services.FilesUploader;
using VideoProjectCore6.Services.Meeting;
using VideoProjectCore6.Services.NotificationService;
using VideoProjectCore6.Services.ParticipantRepository;
using VideoProjectCore6.Services.RecordingService;
using VideoProjectCore6.Services.RoleService;
using VideoProjectCore6.Services.Statistics;
using VideoProjectCore6.Services.TabService;
using VideoProjectCore6.Services.UserService;
using AspNetCoreRateLimit;
using VideoProjectCore6.Utilities.ExternalAPI.Interfaces;
using VideoProjectCore6.Utilities.ExternalAPI;
using VideoProjectCore6.Utility.APIRateLimit;
using VideoProjectCore6.Repositories.IFileRepository;
using VideoProjectCore6.Services.FileService;
using VideoProjectCore6.Repositories.ISmtpConfigRepository;
using VideoProjectCore6.Services.SmtpConfigService;
using VideoProjectCore6.Repositories.IAclRepository;
using VideoProjectCore6.Services.AclRepository;
using VideoProjectCore6.Controllers.Account;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authorization;
using VideoProjectCore6.Utility.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IGeneralRepository, GeneralRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.Services.AddScoped<IFilesUploaderRepository, FilesUploaderRepository>();
builder.Services.AddScoped<IFileConfigurationRepository, FileConfigurationRepository>();
builder.Services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
builder.Services.AddScoped<ISendNotificationRepository, SendNotificationRepository>();
builder.Services.AddScoped<INotificationSettingRepository, NotificationSettingRepository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
//builder.Services.AddScoped<IWorkRepository, WorkRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ITabRepository, TabRepository>();
builder.Services.AddScoped<IConfEventRepository, ConfEventRepository>();
builder.Services.AddScoped<IEventLogRepository, EventLogRepository>();
builder.Services.AddScoped<IRecordingRepository, RecordingRepository>();
builder.Services.AddScoped<ICountryRepsository, CountryRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
//builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IExternalAPIService, ExternalAPIService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<ISmtpConfigRepository, SmtpConfigRepository>();
builder.Services.AddScoped<IAclRepository, AclRepository>();



// Add services to the container.
builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
    .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var currentHostName = builder.Configuration["CONFOMEET_BASE_URL"] ?? "http://localhost:6500";


//builder.Services.AddDbContext<OraDbContext>(options => options.UseOracle(connectionString));
builder.Services.AddDbContext<OraDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration["CONFOMEET_DB_CONNECTION_STRING"]);
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("null",
                "https://localhost:4400",
                "http://localhost:4400",
                "http://localhost:6500",
                "https://lilacmeet.infostrategic.com",
                "https://callpp.infostrategic.com",
                "https://conf.pp.gov.ae",
                "https://call.pp.gov.ae",
                "https://muchat.infostrategic.com",
                currentHostName
                )
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
        });
});

builder.Services.AddControllers();


builder.Services.AddOptions();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, HasPermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionsBasedAuthorizationHandler>();

builder.Services.Configure<ClientRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {

        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/User/signIn",
            Period = "1m",
            Limit = 5,
            QuotaExceededResponse = new QuotaExceededResponse {
            ContentType = "application/json",
            Content = "{{ \"id\": -1, \"result\": false, \"code\": 500, \"message\": [\"quota exceeded. Maximum allowed: {0} per {1}. Please try again in {2} seconds(s).\"] }}",

         },
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/v1/User/Register",
            Period = "1m",
            Limit = 5,
            QuotaExceededResponse = new QuotaExceededResponse {
            ContentType = "application/json",
            Content = "{{ \"id\": -1, \"result\": false, \"code\": 500, \"message\": [\"quota exceeded. Maximum allowed: {0} per {1}. Please try again in {2} seconds(s).\"] }}",

         },
        }
    };
    options.EnableEndpointRateLimiting = true;
});

builder.Services.AddInMemoryRateLimiting();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VideoProject", Version = "v1" });
});




//---------------------token ----------------------------
builder.Services.Configure<ChannelMailFirstSetting>(builder.Configuration.GetSection("ChannelMailFirstSetting"));
builder.Services.Configure<ChannelSMSSetting>(builder.Configuration.GetSection("ChannelSMSSetting"));

var ConfomeetJwtAuthKey = Encoding.ASCII.GetBytes(builder.Configuration["CONFOMEET_AUTH_JWT_KEY"]!);

int passFailureMaxCount = 3;
bool successFailureAttempts = int.TryParse(builder.Configuration["CONFOMEET_LOGIN_ATTEMPS_BEFORE_LOCKOUT"], out int passworFailureAttempts);
if (successFailureAttempts && passworFailureAttempts > 1)
{
    passFailureMaxCount = passworFailureAttempts;
}

int lockedTimeInMinutes = 2;
bool successRead = int.TryParse(builder.Configuration["CONFOMEET_LOCKOUT_TIME_MINUTES"], out int lockedTempTimeInMinutes);
if (successRead && lockedTempTimeInMinutes > 1)
{
    lockedTimeInMinutes = lockedTempTimeInMinutes;
}

var authenticationBuilder = builder.Services.AddAuthentication(au =>
{
    au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

});

authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
{
    jwt.RequireHttpsMetadata = false;
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(ConfomeetJwtAuthKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        // active token expiration.
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true
    };

    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && ((path.StartsWithSegments("/directCall") || path.StartsWithSegments("/eventsStatus") || path.StartsWithSegments("/notifications")))) // for me my hub endpoint is ConnectionHub
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

if (!string.IsNullOrEmpty(builder.Configuration["CONFOMEET_OIDC_AUTHORITY"]))
{
    authenticationBuilder.AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["CONFOMEET_OIDC_AUTHORITY"];
        options.ClientId = builder.Configuration["CONFOMEET_OIDC_CLIENT_ID"];
        options.CallbackPath = VideoProjectCore6.Utility.Uri.CombineUri(OIDCAuthController.ControllerRoute, "/oidc-signin");
        options.MapInboundClaims = false;
        options.SaveTokens = true;
        options.SignOutScheme = IdentityConstants.ApplicationScheme;
        options.SignedOutCallbackPath = VideoProjectCore6.Utility.Uri.CombineUri(OIDCAuthController.ControllerRoute, "/oidc-signout");
    });
}

builder.Services.AddIdentity<User, Role>(opt =>
{
    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockedTimeInMinutes);
    opt.Lockout.MaxFailedAccessAttempts = passFailureMaxCount;
    opt.Password.RequiredLength = 7;
    opt.Password.RequireDigit = false;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = true;
   }).AddEntityFrameworkStores<OraDbContext>()
   .AddDefaultTokenProviders()  // Using Default token provider
   .AddTokenProvider<Token2FAProvider>("mock");


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Admin Panel API", Version = "v1.0" });
    options.DocInclusionPredicate((docName, description) => true);

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // Add this filter as well.
   options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();



// Configure the HTTP request pipeline.
//if (!app.Environment.IsProduction())
//{
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedPrefix | ForwardedHeaders.XForwardedProto
});

app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedPrefix
});
app.UseCors("AllowAll");

app.UseClientRateLimiting();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));
app.UseCors();
app.UseAuthentication();//yhab
app.UseAuthorization();
app.MapControllers();
app.Run();
