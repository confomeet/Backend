namespace VideoProjectCore6.Services
{
    public class Translation
    {

        public static readonly Dictionary<string, string[]> MessageList = new Dictionary<string, string[]>()
           {
            { "eventCanceled",   new string[]   { "لايمكن الانضمام لحدث ملغي", "Can not join cancelled events"}},

            { "OTPUserAccountLocked", new string[]   {"تم تخطي عدد المحاولات المسموح, حسابك مقفل الرجاء مراجعة المدير ", "You’ve reached the maximum attempts. Your account is blocked, please ask the admin. " } } ,
            { "accountRegistered",   new string[]   { "الحساب مسجل في النظام وتم ارسال رابط التفعيل", "Account already exists and registered in the system"} },
            {"oldPasswordNotMatch", new string[] {"كلمة السر القديمة غير صحيحة", "Old password not correct"}},
            {"sessionLifted", new string[] {"رفعت الجلسة", "The session has been lifted"}},

            {"userNotPrison", new string[] {"مستخدم الحالي ليس قسم مسؤول او غير موجود", "Current user is not a prison or not exist"}},

            { "DataRequired",        new string[]   { "من فضللك أدخل جميع المعلومات المطلوبة", "please enter All required data" } },

            { "ENF",        new string[]   {"-3 : Emirates ID is required",   "-3 : Emirates ID is required" } },
            { "sucsessAdd",        new string[]   {"تمت الإضافة بنجاح",   "Added successfully" } },
            { "sucsessUpdate",     new string[]   {"تم التعديل بنجاح",   "Record updated successfully" } },
            { "sucsessDelete",     new string[]   {"تم حذف السجل بنجاح", "record deleted successfully" } },
            { "failedAdd",         new string[]   {"لم تتم إضافة السجل" ,"record insert failed" } },
            { "faildDelete",       new string[]   {"خطأ في حذف السجل" , "Record deleting failed" } },
            { "joinedRecord",      new string[]   {"السجل مرتبط بعملية اخرى" , "record is joined with another process" } },
            { "faildUpdate",       new string[]   {"خطأ في التعديل " ,    " Updating failed" } },
            { "unallowedFileSize", new string[]   { "حجم الملف غير مقبول" , "not allowed file size" } },
            { "unallowedFileType", new string[]   {"نوع ملف غير مسموح به",  "not allowed file type" } },
            { "fileUploaded",      new string[]   {"تم رفع الملف بنجاح",    "File uploaded successfully" } },
            { "UnknownError",      new string[]   {"حدث خطأ ما...أعد المحاولة", "Something went wrong. Please try again later." } },
            { "parentNotFound",    new string[]   {"السجل الأب غير موجود", "parent record not found." } },
            { "repeatedRecord",    new string[]   {"السجل  مكرر", " The record is repeated." } },
            { "fileDeletedFailed", new string[]   {"لم يتم حذف الملف",    "file not deleted!." } },
            { "fileNotFound",      new string[]   {"الملف غير موجود",     "file not found!." } } ,
            { "ServiceNotFound",   new string[]   {"الخدمة غير موجودة",     "service not found!." } } ,
            { "EmailExistedBefore",   new string[]   {"عنوان الايميل موجود مسبقا", "Email address existed before." } } ,
            { "InvalidEmailFormat",   new string[]   {" صيغة عنوان الايميل خاطئة ", "Invalid Email address format" } } ,
            { "InActiveEmail",        new string[]   {"   الايميل غير مفعل ", "Inactive Email address" } } ,
            { "IsActiveEmail",        new string[]   {"  مسبقا الايميل  مفعل ", "active Email address" } } ,
            { "InvalidPhoneNumber",   new string[]   {"رقم الهاتف غير صحيح", "Invalid phone number." } } ,
            { "ExistedPhoneNumber",   new string[]   {"رقم الهاتف  موجود مسبقا", "Existed phone number." } } ,
            { "UserNotExistedBefore", new string[]   {"   المستخدم غير موجود  ", "User not existed." } } ,
            { "UserCreateError", new string[]   {"    خطا في اضافة المستخدم اعد المحاولة بعد قليل  ", "error in adding the user, please try again." } } ,
            { "UserCreated", new string[]   {"تم الانشاء", "Creation is done" } } ,
            { "UserRolesError", new string[]   {"    خطا في اضافة الصلاحيات للمستخدم  ", "error in adding the roles to the user, please try again." } },
            { "UserEmailError", new string[]   {" عنوان الايميل غير موجود يرجى التحقق ", " Email not found, check please. " } } ,
            { "UserAccountLocked", new string[]   {" حسابك مقفل الرجاء مراجعة المدير ", " Your account is blocked, please ask the admin. " } } ,
            { "UserAccountInactive", new string[]   {" يرجى تفعيل الحساب ", " Please activate the account " } } ,
            { "wrongPassword", new string[]   {" كلمة المرور خاطئة يرجى التاكد منها ", " Wrong password, please check. " } } ,
            { "UserNameExistedBefore",new string[]   {"  اسم المستخدم موجود مسبقا ", "User name existed before." } } ,
            { "MissedFullName",       new string[]   {"ب6 محارف على الاقل الرجاء ادخال اسم المعترض ", "Please add your full name 6 character at least." } } ,
            { "EmiratesIDExistedBefore", new string[]   {" الرقم الاماراتي موجود مسبقا", "Emirates id existed before." } } ,
            { "missedParameter",   new string[]   {"بارامتر مفقود",       "Missed parameter!." } },
            { "zeroResult",        new string[]   {"لا يوجد نتائج مطابقة", "No matching result!." } },
            { "wrongParameter",    new string[]   {"بارامتر خاطئ",       "Wrong parameter!." } },
            { "existedBefore",     new string[]   {" قيمة موجودة مسبقا",       " parameter value existed before!." } },
            { "missedLocation",     new string[]   {" موقع غير موجود  ",       " Missed location" } },
            { "errorPayment",     new string[]   { "حدث خطأ غير معروف ", "UNKNOW ERROR!!!!!!" } },
            { "errorAuthentication", new string[]   { "خطا في معلومات تسجيل الدخول ", "Incorrect authentication data." } },
            { "errorConfig",     new string[]   { "خطا في معلومات اعدادات الحساب ", "No connection could be made. Recheck your configuration please." } },
            { "meetingFinished", new string[]   {" الاجتماع بحالة منتهيه",       "The meeting is finished" } },
            { "ParticipantAddFail",    new string[]   {"خطأ بإضافة أحد المشتركين", "Participant Add Fails" } },
            { "PartyUpdateFail", new string[]   {"خطأ بتعديل الطرف", "Party Update Fails" } },
            { "PartyDeleteFail", new string[]   {"خطأ في حذف الطرف", "Party Delete Fails" } },
            { "ExtraAttachmentUpdateFail",    new string[]   {"خطأ بتعديل المرفق الاضافي", "ExtraAttachmentUpdateFail" } },
            { "AttachmentUpdateFail", new string[]   {"خطأ بتعديل المرفق ", "Attachment Update Fail" } },
            { "AttachmentAddFail",    new string[]   {"خطأ بإضافة المرفق ", "Attachment Adding Fail" } },
            { "AttachmentDeleteFail",    new string[]   {"خطأ في حذف المرفق ", "Attachment deleting Fail" } },
            { "AttachmentDeleteDefaultFail",    new string[]   {"خطأ في حذف المرفق الافتراضي ", "Default Attachment deleting Fail" } },
            { "AttachmentAddFailAuth",    new string[]   {"خطأ بإضافة المحضر ", "Attachment Adding Fail" } },
            { "AttachmentFileAddFail",    new string[]   {"خطأ بإضافة الملف المرفق ", "Attachment file Adding Fail" } },
            { "Done",    new string[]   {"تمت العملية بنجاح ", "Done Successfully" } },
            { "DuplicatedEmiratesID", new string[]   {" لا يمكن اضافة طرفان بنفس رقم الهوية الوطني (الرقم الاماراتي)", " adding two parties with same Emarit Id is not acceptance.." } } ,
            { "PartyNotAdded",      new string[]   {"خطـأ في إضافة الطرف",     "Party  not added." } } ,
            { "MissingPartyName",      new string[]   {" الرجاء ادخال اسم الطرف  ",     "Please add the name of the party." } } ,
            { "PartyUserNotAdded",  new string[]   {"خطـأ في إضافة الطرف كمستخدم",     "Party as user  not added." } } ,
            { "MeetingUrl",   new string[]   {"رابط المقابلة : ",     "Meeting Link : " } } ,
            { "Success",      new string[]   {"ناجح", "successful" } } ,
            { "Fail",         new string[]   {"فاشل ", "Unsuccessful" } } ,
            { "FailToken",    new string[]   {"فشل في انشاء توكن  ", "Filed in generating the token" } } ,
            { "MissedGUID",   new string[]   {" توكن مفقود ", "Missed Token" } } ,
            { "ExpiredToken", new string[]   {" الرابط منتهي الصلاحية ", "Expired Token" } } ,
            { "AccessDenied", new string[]   {"   عذرا. ليس لديك صلاحية لذلك ", "Sorry, Access denied for this service." } } ,
            { "NotActivatedToken",new string[]   {" الرابط  غير مفعل بعد ", "The URL is not enabled yet." } } ,
            { "InvalidToken",     new string[]   {" الرابط  غير صالح  ", "The URL is Invalid." } } ,
            { "unauthoraizedForAPP", new string[]   {"  ليس لديك سماحية للوصول الى هذا الطلب  ", "The User is not authorized for the chosen application." } } ,
            { "userHasNotAddresses", new string[]   {"لا يوجد عناوين مرتبطة بالمستخدم لارسال الرمز ", "The User has not addresses to send OTP." } } ,
            { "FiledInsendingOTP",   new string[]   {"خطا في ارسال الرمز الرجاء محاولة الدخول باستخدام حسابك في البوابة الموحدة ", "Failed in sending OTP, please try to log in by your account at undefined gate." } } ,
            { "ExpiredOTP",   new string[]   {"انتهت صلاحية الرمز المرسل, اعد طلب رمز جديد ", "Expired OTP, please renew another OTP" } } ,
            { "IncorrectOTP",   new string[]   {"  الرمز خاطئ يرجى التاكد ", "Incorrect OTP, please review the sending code" } } ,
            { "FailedAddedUser",     new string[]   {" خطا في اضافة المستخدم ", "Failed Added User." } },
            { "missedRoleName",   new string[]   {"اسم الصلاحية مفقود",       "Missed Role name!." } },
            { "duplicatedRoleName",  new string[]   {"اسم الصلاحية مكرر", "The name of the role is existed before!." } },
            { "RoleNotFound",   new string[]   {" الصلاحية غير موجودة ", "The Role is not found!." } },
            { "RoleJoined",   new string[]   {" الصلاحية  مرتبطة باحد المستخدمين يجب ازالتها اولا ", "The Role is joined to user, remove first!." } },
            { "MainRole",   new string[]   {" صلاحية اساسية لايمكن حذفها ", "The Role is minor, unable to delete!." } },
            {"FileNotFound", new string[]{"الملف غير موجود","file not found" } },
            {"NotIdenticalPassword", new string[]{" كلمتا المرور غير متطابقتين "," new password id not identical with the confirmation" } },
            {"PaidSuccess", new string[]{  "تمت العملية بنجاح","operation accomplished successfully" } },//
            {"SessionTimeExpired", new string[]{  "انتهت مدة الجلسة","Session time expired" } },
            {"YES", new string[]{ "نعم", "yes" } },
            {"NO", new string[]{ "لا", "No" } },
            {"NAvailableService", new string[]{ "هذه الخدمة غير متاحة حاليا", "This service is not currently available" } },
            {"OkNotifyParties", new string[]{ "تم إخطار الأطراف", "Parties have been notified" } },
            {"NoNotifyParties", new string[]{ "حصل خطأ في إخطار الأطراف", "There was an error in notifying the parties" } },
            {"partyNotFound", new string[]{ "الطرف غير موجود", "Party not found" } },
            {"NoMatchingRecord", new string[]{ "السجل غير موجود", "No Matching Record" } },
            {"NoMatchingUser", new string[]{ "خطأ في بيانات المستخدم ", "Error in user data" } },
            {"MissingTypeOrId", new string[]{ "يجب تحديد الرقم والنوع لجميع الأطراف", "The number and type must be specified for all participants" } },
            {"MissingEventDate", new string[]{ "يجب تحديد تاريخ بداية ونهاية الحدث", "The start and end date of the event must be specified" } },
            {"ErrorEventDate", new string[]{ "تحقق من صحة تاريخ بداية ونهاية الحدث", "Check that the start and end date of the event are correct" } },
            {"MettingNotFound", new string[]{ "رقم اجتماع غير موجود", "Meeting number not found" } },
            {"MeetingIdGenerateError", new string[]{ "خطأ في إنشاء رقم الاجتماع", "Error creating meeting number" } },
            {"MeetingAddError", new string[]{ "خطأ في إنشاء الاجتماع", "Error creating meeting " } },
            {"NotAvailableDateRange", new string[]{ "المجال الزمني غير متاح", "Time range not available" } },
            {"NoAvailableCabin", new string[]{ "لايوجد كبائن متوفرة", "No available cabins found" } },
            { "EmailError",   new string[]   {"خطأ في الايميل", "Error in Email ." } } ,
            { "EmptyEmail",   new string[]   {"إيميل فارغ", "Empty email" } } ,
            { "SameEmail",   new string[]   { "يجب استخدام بريد إلكتروني فريد لكل مشترك", "A unique email must be used for each participant" } } ,
            { "NoRegisteration",   new string[]   { "عذرا. لا يمكنك التسجيل حاليا", "Sorry . You cannot register at the moment" } } ,
            { "FixSubEventDate",   new string[]   { "تم تعديل تاريخ الأحداث التابعة لهذا الحدث ..يرجى التأكد منها", "The date of the events related to this event has been modified.. Please check it" } } ,
            { "SubEventDateConflict",   new string[]   { "يوجد تعارض مع تاريخ الأحداث الفرعية ..يرجى تصحيحها أولا", "There is a conflict with the date of the sub-events..please correct it first" } } ,
            { "EmailVFD",   new string[]   { "تم التحقق من الايميل", "Your email has been verified" } } ,
            { "AccountACT",   new string[]   { "تم تفعيل الحساب", "Account activated" } } ,
            { "AccountActEr",   new string[]   { "خطأ بتفعيل الحساب", "Error Account activation" } } ,
            { "UserMissingData",   new string[]   { "بيانات المستخدم مفقودة ", "User data missing" } } ,
            { "NotParticipant",   new string[]   { "عذرا ، لا يسمح لك للانضمام إلى هذا الاجتماع", "Sorry, you are not allowed to join this meeting" } } ,
            {"MissingTypeOrUserId", new string[]{ "يجب تحديد رقم المستخدم ونوعه", "User id and type must be specified" } },
            { "UserExisted", new string[]   {"   المستخدم  موجود  ", "User existed." } } ,
            { "ContactNotFound", new string[]   {"جهة الاتصال غير موجودة ", "Contact not found" } } ,
            { "ParticipantAdded", new string[]   {"تمت إضافة الأطراف ", "Participants added" } } ,
            { "ActivateAccount", new string[]   {"تم إرسال رابط تفعيل الحساب إلى بريدك الالكتروني", "An account activation link has been sent to your email" } } ,
            { "RegOk", new string[]   {"تم التسجيل بنجاح", "Successfully registered" } } ,
            //{ "Join", new string[]   {"انضم الآن", "Join now" } } ,
            { "Join", new string[]    {"Join", "Now" } } ,
            { "AccountActivation", new string[]   { "تفعيل الحساب", "Activate account" } } ,
            { "ErrorELog", new string[]   { "خطأ بإضافة السجل الخاص بالحدث", "Error adding event log" } } ,
            { "SameEmiratesId",   new string[]   { "يجب استخدام رقم إماراتي فريد لكل مشترك", "A unique Emirates Id  must be used for each participant" } } ,
            { "SameUUIDId",   new string[]   { "يجب استخدام رقم موحد فريد لكل مشترك", "A unique UUID Id  must be used for each participant" } } ,
            { "SameUserIdentity",   new string[]   { "تعارض في بيانات المستخدم", "User data conflict" } } ,

            { "OTPNotSent",   new string[]   { "خطا اثناء ارسال كود التحقق", "Failed to send OTP" } } ,

            { "ErrorOccured",   new string[]   { "حدث خطا اثناء التحقق", "An error occured while verifing" } } ,

            { "CabinNotExisted",   new string[]   { "كبينة غير موجودة", "Cabinet not existed" } } ,
            { "Charge",   new string[]   { " بتهمة ", " accused of " } } ,
            { "Suspended",   new string[]   { " حسابك معلق ", "Your account is suspended" } } ,

        };

        public static string getMessage(string lang, string key)
        {
            string message;
            try
            {
                message = lang == "ar" ? MessageList[key][0] : MessageList[key][1];
            }
            catch
            {
                message = "key not found";
            }
            return message;
        }
    }
}
