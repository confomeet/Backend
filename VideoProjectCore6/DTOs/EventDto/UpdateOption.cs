namespace VideoProjectCore6.DTOs.CommonDto
#nullable disable
{
    public class UpdateOption
    {
        public bool JustCurrent { get; set; } = false;
        public bool Next { get; set; } = true;
        public bool Previous { get; set; } = false;
        public bool HasOneOption() => Next || Previous;
        public bool HasAllOption() => Next && Previous;
        public short GetCurrentStatus()
        {
            if (JustCurrent) return 0;
            if (Next && Previous) return 2;
            if (Next) return 1;
            if (Previous) return -1;
            return 0;
        }
    }
}
