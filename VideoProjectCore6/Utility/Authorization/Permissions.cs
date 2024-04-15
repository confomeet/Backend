namespace VideoProjectCore6.Utility.Authorization;

public static class Permissions
{
    public const string Meeting_Create = "Meeting.Create";
    public const string Meeting_Search_PartOf = "Meeting.Search.PartOf";
    public const string Meeting_Search_All = "Meeting.Search.All";
    public const string Meeting_Update_Mine = "Meeting.Update.Mine";
    public const string Meeting_Update_All = "Meeting.Update.All";
    public const string Meeting_FetchDetails = "Meeting.FetchDetails";
    public const string Meeting_FetchEventLog_Mine = "Meeting.FetchEventLog.Mine";
    public const string Meeting_FetchEventLog_All = "Meeting.FetchEventLog.All";
    public const string Meeting_Cancel_Mine = "Meeting.Cancel.Mine";
    public const string Meeting_Cancel_All = "Meeting.Cancel.All";
    public const string Meeting_FetchRecordings_All = "Meeting.FetchRecordings.All";
    public const string Meeting_FetchRecordings_IfParticipant = "Meeting.FetchRecordings.IfParticiapnt";
}
