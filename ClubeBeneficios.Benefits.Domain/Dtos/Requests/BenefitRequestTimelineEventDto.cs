using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests
{
    public class BenefitRequestTimelineEventDto
    {
        public Guid Id { get; set; }
        public Guid BenefitRequestId { get; set; }

        public string EventType { get; set; } = string.Empty;
        public string? EventStatus { get; set; }
        public string? EventPoint { get; set; }
        public string? EventDescription { get; set; }

        public Guid? ActorUserId { get; set; }
        public string? ActorName { get; set; }

        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
