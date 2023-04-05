using System;
using VideoProjectCore6.Models;
#nullable disable
namespace VideoProjectCore6.DTOs.EventDto
{
    public class EventGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Type { get; set; }
        public string Description { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
        public string Organizer { get; set; }
        public string MeetingId { get; set; }
        public bool Invitation { get; set; }
        public int? ParentEventId { get; set; }
        public int? OrderNo { get; set; }
        public string TimeZone { get; set; }
        public sbyte? Status { get; set; }

        public bool? AllDay { get; set; }
        public EventGetDto ParentEvent { get; set; }


        public EventGetDto()
        {

        }

        public static EventGetDto GetDTO(Event event_)
        {
            EventGetDto dto = new EventGetDto
            {
                Id = event_.Id,
                //UserId = event_.UserId,
                StartDate = event_.StartDate,
                EndDate = event_.EndDate,
                Description = event_.Description,
                Organizer= event_.Organizer,
                Type = event_.Type,
                Topic = event_.Topic,
                SubTopic= event_.SubTopic,
                MeetingId = event_.MeetingId,
                ParentEventId = event_.ParentEvent,
                TimeZone = event_.TimeZone,
                OrderNo=event_.OrderNo,
                Status=event_.RecStatus
            };
            return dto;
        }
    }
}
