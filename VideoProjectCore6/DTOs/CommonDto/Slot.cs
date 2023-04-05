using System.Linq;

namespace VideoProjectCore6.DTOs.CommonDto
#nullable disable
{
    public class Slot : DateTimeRange
    {
        public bool IsContainItByStart(DateTimeRange dt)
        {
            return StartDateTime == dt.StartDateTime && EndDateTime >= dt.EndDateTime;
        }
        public bool IsContainItByEnd(DateTimeRange dt)
        {
            return EndDateTime == dt.EndDateTime && StartDateTime <= dt.StartDateTime;
        }
        public bool IsStartBeforEnd(DateTimeRange dt)
        {
            return dt.StartDateTime > StartDateTime && dt.StartDateTime < EndDateTime;
        }
        public void TrimStart(ushort minutes)
        {
            StartDateTime = StartDateTime.AddMinutes(-1 * minutes);
        }
        public List<DateTime> SplitRange(ushort minutes)
        {
            var result = new List<DateTime>();
            var slotsCount = (EndDateTime - StartDateTime).TotalMinutes / minutes;
            for (var i = 0; i < slotsCount; i++)
            {
                var slotStart = StartDateTime.AddMinutes(i * minutes);
                result.Add(slotStart);
            }
            return result;
        }
        public List<DateTimeRange> GetSlotsOfRange(ushort slotMinutes)
        {
            var startTime = StartDateTime;
            var slots = new List<DateTimeRange>();
            if(startTime.AddMinutes(slotMinutes) > EndDateTime)
            {
                return slots;
            }
            while (startTime.AddMinutes(slotMinutes) <= EndDateTime)
            {
                slots.Add(new DateTimeRange
                {
                    StartDateTime = startTime,
                    EndDateTime = startTime.AddMinutes(slotMinutes)
                });
                startTime = startTime.AddMinutes(slotMinutes);
            }
            return slots;
        }
        public List<MarkedSlote> GetMarkedSlots(ushort sloteMinutes)
        {
            var markedSlots = new List<MarkedSlote>();
            var startTime = StartDateTime;
            while (startTime < EndDateTime)
            {
                markedSlots.Add(new MarkedSlote { SlotStartTime = startTime, active = 1 });
                startTime = startTime.AddMinutes(sloteMinutes);
            }
            return markedSlots;
        }
    }
}
