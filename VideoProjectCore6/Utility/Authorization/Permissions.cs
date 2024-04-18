namespace VideoProjectCore6.Utility.Authorization;

public static class Permissions
{
    public const string Group_Create = "Group.Create";
    public const string Group_Read = "Group.Read";
    public const string Group_Update = "Group.Update";
    public const string Group_Delete = "Group.Delete";
    public const string Meeting_Create = "Meeting.Create";
    public const string Meeting_Search_IfParticipant = "Meeting.Search.IfParticipant";
    public const string Meeting_Search_All = "Meeting.Search.All";
    public const string Meeting_Update_Mine = "Meeting.Update.Mine";
    public const string Meeting_Update_All = "Meeting.Update.All";
    public const string Meeting_FetchEventLog_Mine = "Meeting.FetchEventLog.Mine";
    public const string Meeting_FetchEventLog_All = "Meeting.FetchEventLog.All";
    public const string Meeting_Cancel_Mine = "Meeting.Cancel.Mine";
    public const string Meeting_Cancel_All = "Meeting.Cancel.All";
    public const string Profile_EditPassword = "Profile.EditPassword";
    public const string Profile_Update = "Profile.Update";
    public const string Profile_Read = "Profile.Read";
    public const string SmtpConfig_CreateUpdate = "SmtpConfig_CreateUpdate";
    public const string SmtpConfig_Read = "SmtpConfig_Read";
    public const string SystemStats_Read = "SystemStats_Read";
    public const string User_Create = "User.Create";
    public const string User_Delete = "User.Delete";
    public const string User_Disable = "User.Disable";
    public const string User_Enable = "User.Enable";
    public const string User_ForceConfirmEmail = "User.ForceConfirmEmail";
    public const string User_Read = "User.Read";
    public const string User_SearchById = "User.SearchById";
    public const string User_Update = "User.Update";
}
