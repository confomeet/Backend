namespace VideoProjectCore6.Services
{
    public class Translation
    {

        public static readonly Dictionary<string, string[]> MessageList = new Dictionary<string, string[]>()
           {
            { "eventCanceled",   new string[]   { "Невозможно присоединиться к завершенной конференции", "Can not join cancelled events"}},
            { "OTPUserAccountLocked", new string[]   {"Аккаунт пользователя заблокирован", "You’ve reached the maximum attempts. Your account is blocked, please ask the admin. " } } ,
            { "accountRegistered",   new string[]   { "Аккаунт уже зарегистрирова", "Account already exists and registered in the system"} },
            {"sessionLifted", new string[] {"The session has been lifted", "The session has been lifted"}},
            { "sucsessAdd",        new string[]   {"تمت الإضافة بنجاح",   "Added successfully" } },
            { "sucsessUpdate",     new string[]   {"Запись бновлена",   "Record updated successfully" } },
            { "sucsessDelete",     new string[]   {"Запись удалена", "record deleted successfully" } },
            { "failedAdd",         new string[]   {"не удалось добавить запись" ,"record insert failed" } },
            { "faildDelete",       new string[]   {"Не удалось удалить запись", "Record deleting failed" } },
            { "faildUpdate",       new string[]   {"Не удалось обновить запись" , " Updating failed" } },
            { "UnknownError",      new string[]   {"Произошла ошибка, поробуйте позже", "Something went wrong. Please try again later." } },
            { "EmailExistedBefore",   new string[]   {"Адрес эл. почты уже зарегистрирован", "Email address existed before." } } ,
            { "InvalidEmailFormat",   new string[]   {"Неверный формат эл. почты", "Invalid Email address format" } } ,
            { "InActiveEmail",        new string[]   {"Неактивный адерс эл. почты", "Inactive Email address" } } ,
            { "IsActiveEmail",        new string[]   {"Аккаунт уже активирован", "active Email address" } } ,
            { "InvalidPhoneNumber",   new string[]   {"Неправильный номер телефона", "Invalid phone number." } } ,
            { "ExistedPhoneNumber",   new string[]   {"Пользователь с данным телефоном уже зарегистрирован", "Existed phone number." } } ,
            { "UserNotExistedBefore", new string[]   {"Пользователь не существует", "User not existed." } } ,
            { "UserCreateError", new string[]   {"Не удалось создать пользователя", "error in adding the user, please try again." } } ,
            { "UserCreated", new string[]   {"Пользователь создан", "Creation is done" } } ,
            { "UserRolesError", new string[]   {"Не удалось добавить пользователю роль", "error in adding the roles to the user, please try again." } },
            { "UserEmailError", new string[]   {"Указанный адрес эл. почты не найден", " Email not found, check please. " } } ,
            { "UserAccountLocked", new string[]   {"Аккаунт пользователя заблокирован, обратитесь к администратору", " Your account is blocked, please ask the admin. " } } ,
            { "UserAccountInactive", new string[]   {"Требуется активация аккаунта", " Please activate the account " } } ,
            { "wrongPassword", new string[]   {"Пароль неверный", " Wrong password, please check. " } } ,
            { "UserNameExistedBefore",new string[]   {"Пользователь с таким данными уже существует", "User name existed before." } } ,
            { "missedParameter",   new string[]   {"Не хватает параметра. ",       "Missed parameter!." } },
            { "zeroResult",        new string[]   {"Результат не найден", "No matching result!." } },
            { "wrongParameter",    new string[]   {"Некорректный параметр",       "Wrong parameter!." } },
            { "errorAuthentication", new string[]   { "خطا في معلومات تسجيل الدخول ", "Incorrect authentication data." } },
            { "meetingFinished", new string[]   {" الاجتماع بحالة منتهيه",       "The meeting is finished" } },
            { "PartyDeleteFail", new string[]   {"خطأ في حذف الطرف", "Party Delete Fails" } },
            { "Done",    new string[]   {"Успешно", "Done Successfully" } },
            { "MeetingUrl",   new string[]   {"Ссылка на конференцию: ",     "Meeting Link : " } } ,
            { "Success",      new string[]   {"Успешно", "successful" } } ,
            { "userHasNotAddresses", new string[]   {"Данному пользователю не возможно отправить одноразовый код", "The User has not addresses to send OTP." } } ,
            { "FiledInsendingOTP",   new string[] { "Не удалось отправить одноразовый код", "Failed in sending OTP, please try to log in by your account at undefined gate." } } ,
            { "ExpiredOTP",   new string[]   {"Истекший одноразовый код", "Expired OTP, please renew another OTP" } } ,
            { "IncorrectOTP",   new string[]   {"Неправильный однаразовый код", "Incorrect OTP, please review the sending code" } } ,
            { "InvitationNotSent", new string[] {"Не удалось отправить приглашение в конференцию", "Failed to send invitation"} },
            { "InvitationSent", new string[] {"Участнику отправлено приглашение в конференцию", "Invitation sent to participant"} },
            { "FailedAddedUser",     new string[]   {"Не удалось добавить пользователя", "Failed Added User." } },
            { "missedRoleName",   new string[]   {"Не указано название роли",       "Missed Role name!." } },
            { "duplicatedRoleName",  new string[]   {"Роль с таким именем уже сущесвует", "The name of the role is existed before!." } },
            { "RoleNotFound",   new string[]   {"Роль не найден", "The Role is not found!." } },
            { "RoleJoined",   new string[]   {"Не возможно удалить роль, пока она назначена другим пользователям", "The Role is joined to user, remove first!." } },
            { "MainRole",   new string[]   {"Не удалось удалить роль", "The Role is minor, unable to delete!." } },
            {"FileNotFound", new string[]{"Файл не найден","file not found" } },
            {"NotIdenticalPassword", new string[]{"Пароли не совпадают"," new password id not identical with the confirmation" } },
            {"partyNotFound", new string[]{ "Участник не найден", "Party not found" } },
            {"NoMatchingRecord", new string[]{ "Запись не найдена", "No Matching Record" } },
            {"MissingEventDate", new string[]{ "Не указаны время начала и окончания конференции", "The start and end date of the event must be specified" } },
            {"ErrorEventDate", new string[]{ "Проверьте корректность времени начала и конфа конференции", "Check that the start and end date of the event are correct" } },
            {"MettingNotFound", new string[]{ "Конференция не найдена", "Meeting number not found" } },
            {"MeetingAddError", new string[]{ "Не удалось создать конференцию", "Error creating meeting " } },
            {"NotAvailableDateRange", new string[]{ "Заданный временной период недоступен", "Time range not available" } },
            {"NoAvailableCabin", new string[]{ "Не найден свободный кабинет", "No available cabins found" } },
            { "EmailError",   new string[]   {"Неправильный формат эл. почты", "Error in Email ." } } ,
            { "EmptyEmail",   new string[]   {"Не указана эл. почта", "Empty email" } } ,
            { "SameEmail",   new string[]   { "У каждого участника должны быть уникальны email", "A unique email must be used for each participant" } } ,
            { "NoRegisteration",   new string[]   { "Извините, регистрация сейчас не возможна", "Sorry . You cannot register at the moment" } } ,
            { "FixSubEventDate",   new string[]   { "Даты событий изменены, проверьте корректность", "The date of the events related to this event has been modified.. Please check it" } } ,
            { "SubEventDateConflict",   new string[]   { "Конфликт в датах между событием и его родителем", "There is a conflict with the date of the sub-events..please correct it first" } } ,
            { "EmailVFD",   new string[]   { "Эл. адрес", "Your email has been verified" } } ,
            { "AccountACT",   new string[]   { "Аккаунт активирован", "Account activated" } } ,
            { "AccountActEr",   new string[]   { "Не удалось активировть аккаунт", "Error Account activation" } } ,
            { "UserMissingData",   new string[]   { "Данные пользователя не найдены", "User data missing" } } ,
            { "NotParticipant",   new string[]   { "К сожалению, вы не можете присоединиться к этой конференции", "Sorry, you are not allowed to join this meeting" } } ,
            { "UserExisted", new string[]   {"Такой пользователь уже существует", "User existed." } } ,
            { "ContactNotFound", new string[]   {"Контакт не найден", "Contact not found" } } ,
            { "ParticipantAdded", new string[]   {"Участник добавлен", "Participants added" } } ,
            { "ActivateAccount", new string[]   {"Ссылка для активации аккаунта отправлена на эл. почту", "An account activation link has been sent to your email" } } ,
            { "RegOk", new string[]   {"Регистрация прошла успешно", "Successfully registered" } } ,
            { "Join", new string[]    {"Присоединиться", "Join" } } ,
            { "AccountActivation", new string[]   { "Активация аккаунта", "Activate account" } } ,
            { "ErrorELog", new string[]   { "Ошибка при добавлении лога события", "Error adding event log" } } ,
            { "SameUUIDId",   new string[]   { "UUID должен быть уникальным", "A unique UUID Id  must be used for each participant" } } ,
            { "SameUserIdentity",   new string[]   { "Такой пользователь уже существует", "User data conflict" } } ,

            { "OTPNotSent",   new string[]   { "Не удалось отправить одноразовый код", "Failed to send OTP" } } ,

            { "ErrorOccured",   new string[]   { "Произошла ошибка", "An error occured while verifing" } } ,

            { "CabinNotExisted",   new string[]   { "Кабинет не найден", "Cabinet not existed" } } ,
            { "Charge",   new string[]   { " accused of ", " accused of " } } ,
            { "Suspended",   new string[]   { "Аккаунт отключен", "Your account is suspended" } } ,
            { "PasswordIsRequired", ["Отсутсвует пароль", "Password is required"] },
            { "Room", [ "Комната", "Room"] },
            { "UserNotFound", ["Пользователь не найден", "User not found"]}

        };

        public static string getMessage(string lang, string key)
        {
            string message;
            try
            {
                message = lang == "ru" ? MessageList[key][0] : MessageList[key][1];
            }
            catch
            {
                message = key;
            }
            return message;
        }
    }
}
