
#nullable enable
using System;

namespace StudioSchedule.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class OverlappingSessionException : DomainException
    {
        public OverlappingSessionException(string message) : base(message) { }
    }

    public class InvalidSessionParticipantsException : DomainException
    {
        public InvalidSessionParticipantsException(string message) : base(message) { }
    }

    public class InvalidDateRangeException : DomainException
    {
        public InvalidDateRangeException(string message) : base(message) { }
    }
}