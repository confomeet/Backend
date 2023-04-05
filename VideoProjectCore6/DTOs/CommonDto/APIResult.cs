#nullable disable
namespace VideoProjectCore6.DTOs.CommonDto
{
    public class APIResult
    {
        public int Id { get; set; } = -1;
        public dynamic Result { get; set; } = null;
        public RESPONSE_CODE Code { get; set; } = RESPONSE_CODE.ERROR;
        public List<string> Message { get; set; } = new List<string>();
        // reference https://restfulapi.net/http-status-codes/#4xx.
        public enum RESPONSE_CODE
        {
            OK = 200,
            CREATED = 201,
            BadRequest = 400,
            Locked = 423,
            NotAcceptable = 406,
            UnavailableForLegalReasons = 451,
            NoResponse = 444,
            ERROR = 500,
            NotImplemented = 501,
            PageNotFound = 404
        }
        public APIResult()
        {

        }
        public APIResult(int id, dynamic result, RESPONSE_CODE code, List<string> messages)
        {
            Id = id;
            Result = result;
            Code = code;
            Message = messages;
        }

        public APIResult SuccessMe(int? id, string msg="", bool clearMsg = false, RESPONSE_CODE code = RESPONSE_CODE.OK, dynamic result=null)
        {
            Id = (int)(id != null ? id : 1);
            if (clearMsg)
            {
                Message.Clear();
            }
            Message.Add(msg);
            Result = result==null ? true:result;
            Code = code;
            return this;
        }

        public APIResult FailMe(int id, string msg="", bool clearMsg = false, RESPONSE_CODE code = RESPONSE_CODE.ERROR, dynamic result = null)
        {
            Id = id < 0 ? id : -1;
            if (clearMsg)
            {
                Message.Clear();
            }
            Message.Add(msg);
            Result =  result == null ? false : result;
            Code = code;
            return this;
        }
    }
}
