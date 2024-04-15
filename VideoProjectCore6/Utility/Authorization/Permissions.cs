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
    public const string Meeting_FetchDetails_IfParticipant = "Meeting.FetchDetails.IfParticipant";
    public const string Meeting_FetchDetails_All = "Meeting.FetchDetails.All";
    public const string Meeting_FetchEventLog_Mine = "Meeting.FetchEventLog.Mine";
    public const string Meeting_FetchEventLog_All = "Meeting.FetchEventLog.All";
    public const string Meeting_Cancel_Mine = "Meeting.Cancel.Mine";
    public const string Meeting_Cancel_All = "Meeting.Cancel.All";
    public const string Meeting_FetchRecordings_All = "Meeting.FetchRecordings.All";
    public const string Meeting_FetchRecordings_IfParticipant = "Meeting.FetchRecordings.IfParticiapnt";
    public const string Profile_EditPassword = "Profile.EditPassword";
    public const string Profile_Update = "Profile.Update";
    public const string Profile_Read = "Profile.Read";
    public const string User_Configure2FA_All = "User.Configure2FA.All";
    public const string User_Configure2FA_Self = "User.Configure2FA.Self";
    public const string User_Create = "User.Create";
    public const string User_Delete = "User.Delete";
    public const string User_Disable = "User.Disable";
    public const string User_Enable = "User.Enable";
    public const string User_ForceConfirmEmail = "User.ForceConfirmEmail";
    public const string User_Read = "User.Read";
    public const string User_ResetPassword = "User.ResetPassword";
    public const string User_SearchById = "User.SearchById";
    public const string User_Update = "User.Update";
}
