#nullable disable
namespace VideoProjectCore6.Services
{
    public class Constants
    {

        // constants for policy
        public const string AdminPolicy = "admin";
        public const string DefaultUserPolicy = "user";
        public const string EmployeePolicy = "employee";
        public const string DefaultVisitorPolicy = "visitor";

        public const string INVALID_EMAIL_SUFFIX = "@invalidEmail.com";
        public const string INVALID_EMAIL_PREFIX = "invalidEmail";

        // Secret constants.
        public const string otpBase32Secret = "6L4OH6DDC4PLNQBA5422GM67KXRDIQQP";
        public const string OTP_TITLE_AR = "نظام ليلك";
        public const string OTP_TITLE_EN = "Lilac system ";
        public const string OTP_BODY_AR = "كود التحقق: ";
        public const string OTP_BODY_EN = " OTP code: ";
        public const int OTP_PERIOD_If_MISSED_IN_APP_SETTING = 60;

        public const string CANCEL_BODY_AR = " ";
        public const string CANCEL_BODY_EN = "Meeting has been canceled: ";

        // User types
        public const string ADMIN = "1";
        public const string USER = "2";
        public const string VISITOR = "22";
        public const string POLICE_STATION = "3";
        public const string CABIN = "4";

        public const string PROSODY_EVENT_ROOM_CREATED = "room_created";
        public const string PROSODY_EVENT_OCCUPANT_JOINED = "occupant_joined";
        public const string PROSODY_EVENT_OCCUPANT_LEAVING = "occupant_leaving";
        public const string PROSODY_EVENT_ROOM_DESTROYED = "end_call_for_all";
        public const string PROSODY_EVENT_ROOM_FINISHED = "room_destroyed"; //"room_destroyed"
        public const string PROSODY_EVENT_USER_LEAVING_LOBBY = "occupant_leaving_lobby";

        // constants for the maximum number of fault attempts to send notification.
        public const int MAX_NOTIFY_SEND_ATTEMPTS = 3;

        // constants for tables names which contains shortcut fields.
        // tables.
        public const string SERVICE_KIND = "service_kind";
        public const string WORKING_HOURS = "working_hours";
        public const string DAY_OFF = "dayOff";
        public const string NOTIFICATION_TEMPLATE = "noti_temp";
        public const string NOTIFICATION_TEMPLATE_DETAIL = "noti_temp_detail";
        public const string ROLE = "role";
        public const string TAB = "tab";
        public const string TRANSACTION_FEE = "transaction_fee";
        public const string LOCATION = "location";
        public const string DAY_OFF_REASON_SHORTCUT = "ReasonShortcut";
        // tables inside lookupType.
        public const string NOTIFICATION_CHANNEL = "notification_channel";
        public const string NOTIFICATION_MAIL_CHANNEL = "channel_mail";  // TODO rename to first channel and add second accord to Figma design.
        public const string NOTIFICATION_SMS_CHANNEL = "channel_sms";
        public const string NOTIFICATION_INTERNAL_CHANNEL = "channel_internal";


        public const string TAB_NAME_SHORTCUT = "TabNameShortcut";
        public const string NOTIFICATION_TEMPLATE_NAME_SHORTCUT = "NShort";
        public const string NOTIFICATION_TEMPLATE_DETAIL_TITLE_SHORTCUT = "TShort";
        public const string NOTIFICATION_TEMPLATE_DETAIL_BODY_SHORTCUT = "BShort";
        public const string ROLE_NAME_SHORTCUT = "roleNameShort";


        // Contact types
        public const string INDIVIDUAL = "individual";
        public const string COMPANY = "company";
        public const string SECTION = "section";
        public const string MANAGER = "manager";
        public enum MEETING_STATUS
        {
            FINISHED = -1,
            PENDING = 0,
            STARTED = 1
        }

        public enum NOTIFICATION_STATUS
        {
            PENDING = 0,
            SENT = 1,
            ERROR = 2,
        }

        public enum PROFILE_STATUS
        {
            ENABLED = 1,
            DISABLED = 0,
            BLOCKED = 2,
            SUSPENDED = 3
        }

        public enum CABIN_STATUS
        {
            ENABLED = 1,
            DISABLED = 0,
            BLOCKED = 2,
            SUSPENDED = 3
        }

        public enum EVENT_STATUS
        {
            CANCELED = -2,
            DELETED = -1,
            SUSPEND = 0,
            ACTIVE = 1,
            RESCHEDULED = 2
        };

        public static Dictionary<EVENT_STATUS, Dictionary<string, string>> EventStatusValue = new Dictionary<EVENT_STATUS, Dictionary<string, string>>()
        {
            { EVENT_STATUS.CANCELED, new Dictionary<string, string>(){{ "ar","ملغي"},{"en","Canceled"} }},
            { EVENT_STATUS.DELETED, new Dictionary<string, string>(){{ "ar","محذوف"},{"en","Deleted"} }},
            { EVENT_STATUS.SUSPEND, new Dictionary<string, string>(){{ "ar","معلق "},{"en", "Suspend" } }},
            { EVENT_STATUS.ACTIVE, new Dictionary<string, string>(){{ "ar","فعال "},{"en","Active"} }},
            { EVENT_STATUS.RESCHEDULED, new Dictionary<string, string>(){{ "ar","معاد جدولته"},{"en", "Rescheduled" } }},
        };

        public static Dictionary<EVENT_TYPE, string[]> MeetingStatusValue = new Dictionary<EVENT_TYPE, string[]>()
        {
            { EVENT_TYPE.EVENT_TYPE_CONF_COMING,  new string[]  {"القادم" , "Incoming" } },
            { EVENT_TYPE.EVENT_TYPE_CONF_FINISHED,  new string[]  { "تم انهاء الاجتماع","Call ended" } },
            { EVENT_TYPE.EVENT_TYPE_CONF_STARTED,  new string[]  { "نشط", "Active"} },


            { EVENT_TYPE.EVENT_TYPE_CONF_USER_JOIN,  new string[]  { "انضم","User Joined" } },
            { EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE,  new string[]  { "غادر", "User left"} },
            { EVENT_TYPE.EVENT_TYPE_CONF_USER_LEAVE_LOBBY,  new string[]  { "خروج الى اللوبي", "Left to lobby"} }

        };


        public enum EVENT_TYPE : int
        {
            EVENT_TYPE_CONF_CREATED = 0,
            EVENT_TYPE_CONF_STARTED = 1,
            EVENT_TYPE_CONF_FINISHED = 2,
            EVENT_TYPE_CONF_COMING = 3,
            EVENT_TYPE_CONF_USER_JOIN = 4,
            EVENT_TYPE_CONF_USER_LEAVE = 5,
            EVENT_TYPE_CONF_USER_LEAVE_LOBBY = 6,
        }

        public const string NAME = "Name";
        public const string FROM_DATE = "FromDate";
        public const string TO_DATE = "ToDate";
        public const string FROM_TIME = "FromTime";
        public const string TO_TIME = "ToTime";
        public const string TIME = "Time";
        public const string LINK = "Link";
        public const string LINK_MOB = "Link_Mob";
        public const string TOPIC = "Topic";
        public const string DESCRIPTION = "Description";
        public const string TIMEZONE = "TimeZone";
        public const string MEETING_ID = "MeetingId";
        public const string CHARGE = "Charge";
        public const string PASSCODE = "PassCode";
        public const string NOTE = "Note";




        public static readonly Dictionary<string, string> ParameterDic = new Dictionary<string, string>()
        {
            {FROM_DATE, "Date" },
            {TO_DATE, "Date" },
            {FROM_TIME, "Time" },
            {TO_TIME, "Time" },
            {TIME, "Time" },
            {NAME, "string" },
            {LINK, "string" },
            {LINK_MOB, "string" },
            {TOPIC, "string" },
            {DESCRIPTION, "string" },
            {TIMEZONE, "string" },
            {PASSCODE, "string" },
        };
        public static string ReplaceParemeterByValues(Dictionary<string, string> parameterDic, string text)
        {
            foreach (var para in parameterDic)
            {
                if (text.Contains('@' + para.Key))
                {
                    text = text.Replace('@' + para.Key, para.Value);
                }
            }
            return text;
        }
        public const string EMAIL_BODY_NEWLINE = ",";
        public const string INVITATION_TEMPLATE = "en-invitation.html";
        public const string ACTIVATION_TEMPLATE = "activation.html";
        public const string UNSUBSCRIBE_TEMPLATE = "unsubscribe.html";
        public const string DEFAULT_TEMPLATE = "default.html";

        public const string OPJECT_TYPE_EVENT = "EVT";
        public const string OPJECT_TYPE_PARTICIPANT = "PRT";

        public const string UPDATE_EVENT_ACTION = "UPDATE_EVENT";
        public const string DELETE_PARTICIPANT_ACTION = "DELETE_PARTICIPANT";
        public const string ADD_PARTICIPANT_ACTION = "ADD_PARTICIPANT";
        public const string UPDATE_PARTICIPANT_ACTION = "UPDATE_PARTICIPANT";
        public const string SEND_INVITATION_ACTION = "SEND_INVITATION";
        public const string CANCEL_EVENT_ACTION = "CANCEL_EVENT";

        public const string PROTOCOL_POST = "POST";
        public const string PROTOCOL_GET = "GET";
        public const string DEFAULT_LANG = "ar";

        // contact tabs
        public enum CONTACT_UPPER_TABS
        {
            //INBOX = 1,
            //OUTBOX = 2,
            ORGANIZATION = 2,
            MYCONTACT = 1,
            ALL = 0,
        };

        public static Dictionary<CONTACT_UPPER_TABS, Dictionary<string, string>> UpperTabsValues = new Dictionary<CONTACT_UPPER_TABS, Dictionary<string, string>>()
        {
            //{ CONTACT_UPPER_TABS.OUTBOX, new Dictionary<string, string>(){{ "ar","صادر"},{"en","Outbox"} } },
            //{ CONTACT_UPPER_TABS.INBOX, new Dictionary<string, string>(){{ "ar","وارد"},{"en","Inbox"} }} ,
            { CONTACT_UPPER_TABS.ORGANIZATION, new Dictionary<string, string>(){{ "ar","جهات اتصال الشركة"},{"en","Organization contacts"} }},
            { CONTACT_UPPER_TABS.MYCONTACT, new Dictionary<string, string>(){{ "ar", "جهات الاتصال" },{"en","My Contacts"} }},
            { CONTACT_UPPER_TABS.ALL, new Dictionary<string, string>(){{ "ar","الكل"},{"en","All"} }},
        };
        public static Dictionary<byte, Dictionary<string, string>> DaysOfWeek = new Dictionary<byte, Dictionary<string, string>>
        {
         { 0, new Dictionary<string, string> { { "ar", "الإثنين"  } , { "en", "Monday"  } } },
         { 1, new Dictionary<string, string> { { "ar", "الثلاثاء" } , { "en", "Tuesday" } } },
         { 2, new Dictionary<string, string> { { "ar", "الأربعاء" } , { "en", "Wednesday" } } },
         { 3, new Dictionary<string, string> { { "ar", "الخميس"  } , { "en", "Thursday" } } },
         { 4, new Dictionary<string, string> { { "ar", "الجمعة"  } , { "en", "Friday" } } },
         { 5, new Dictionary<string, string> { { "ar", "السبت"   } , { "en", "Saturday" } } },
         { 6, new Dictionary<string, string> { { "ar", "الأحد"    } , { "en", "Sunday" } } }
        };
        public static string GetDayOfWeek(string language, byte dayNumber)
        {
            string value;
            if (DaysOfWeek.TryGetValue(dayNumber, out var dayDict))
            {
                if (dayDict.TryGetValue(language, out value))
                {
                    return value;
                }
                else
                {
                    throw new ArgumentException("Language not found in dictionary");
                }
            }
            else
            {
                throw new ArgumentException("Day number not found in dictionary");
            }
        }
        public static Dictionary<string, string> GetDayOfWeek(byte dayNumber)
        {
            if (DaysOfWeek.TryGetValue(dayNumber, out var dayDict))
            {
                return DaysOfWeek[dayNumber];
            }
            else
            {
                throw new ArgumentException("Day number not found in dictionary");
            }
        }
    }
}